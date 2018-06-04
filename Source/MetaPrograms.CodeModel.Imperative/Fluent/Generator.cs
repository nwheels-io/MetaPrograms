using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using MetaPrograms.CodeModel.Imperative.Statements;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

// ReSharper disable InconsistentNaming

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public static class Generator
    {
        public static void NAMESPACE(string name, Action body)
        {
            using (GetContextOrThrow().PushState(new NamespaceContext(name)))
            {
                body();
            }
        }

        public static FluentVisibility PUBLIC => new FluentVisibility(MemberVisibility.Public);
        public static FluentVisibility PRIVATE => new FluentVisibility(MemberVisibility.Private);
        public static FluentVisibility PROTECTED => new FluentVisibility(MemberVisibility.Protected);
        public static FluentVisibility INTERNAL => new FluentVisibility(MemberVisibility.Internal);

        public static void ATTRIBUTE<T>(params object[] constructorArgumentsAndBody)
        {
            ATTRIBUTE(GetContextOrThrow().FindType<T>(), constructorArgumentsAndBody);
        }

        public static void ATTRIBUTE(MemberRef<TypeMember> type, params object[] constructorArgumentsAndBody)
        {
            var context = GetContextOrThrow();
            var attribute = FluentHelpers.BuildAttribute(context, type, constructorArgumentsAndBody);

            if (context.TryPeekState<ParameterContext>(out var parameterContext))
            {
                parameterContext.Attributes = parameterContext.Attributes.Add(attribute);
            }
            else if (context.TryPeekState<TypeMemberBuilder>(out var builder))
            {
                builder.Attributes.Add(attribute);
            }
            else
            {
                var member = context.GetCurrentMember();
                member.WithAttributes(member.Attributes.Add(attribute), shouldReplaceSource: true);
            }
        }

        public static void NAMED(string name, object value)
        {
            var context = GetContextOrThrow(); 
            
            context
                .PeekStateOrThrow<AttributeContext>()
                .NamedProperties
                .Add(new NamedPropertyValue(name, context.GetConstantExpression(value)));
        }

        public static void EXTENDS<T>()
        {
            EXTENDS(GetContextOrThrow().FindType<T>());
        }

        public static void EXTENDS(TypeMember type)
        {
            var typeBuilder = GetContextOrThrow().PeekStateOrThrow<TypeMemberBuilder>();
            typeBuilder.BaseType = type.GetRef();
        }

        public static void IMPLEMENTS<T>()
        {
            IMPLEMENTS(GetContextOrThrow().FindType<T>());
        }

        public static void IMPLEMENTS(TypeMember type)
        {
            var typeBuilder = GetContextOrThrow().PeekStateOrThrow<TypeMemberBuilder>();
            typeBuilder.Interfaces.Add(type.GetRef());
        }

        public static void PARAMETER<T>(string name, out MethodParameter @ref, Action body = null)
        {
            PARAMETER(GetContextOrThrow().FindType<T>(), name, out @ref, body);
        }

        public static void PARAMETER(TypeMember type, string name, out MethodParameter @ref, Action body = null)
        {
            var context = GetContextOrThrow();
            var method = (MethodMemberBase)context.GetCurrentMember();
            var parameterContext = new ParameterContext();

            using (context.PushState(parameterContext))
            {
                body?.Invoke();
            }

            var newParameter = new MethodParameter(
                name, 
                method.Signature.Parameters.Count,
                type.GetRef(),
                parameterContext.Modifier,
                parameterContext.Attributes);

            var newSignature = method.Signature.AddParameter(newParameter);
            method = method.WithSignature(newSignature, shouldReplaceSource: true);

            @ref = newParameter;
        }

        public static void LOCAL(MemberRef<TypeMember> type, string name, out LocalVariable @ref)
        {
            @ref = new LocalVariable(name, type);
            var block = BlockContext.GetBlockOrThrow();
            block.AddLocal(@ref);
            block.AppendStatement(new VariableDeclarationStatement(@ref, initialValue: null));
        }

        public static void LOCAL<T>(string name, out LocalVariable @ref)
        {
            var type = GetContextOrThrow().FindType<T>();
            LOCAL(type, name, out @ref);
        }

        public static void GET() { }
        public static void GET(Action body) { }
        public static void SET(Action<LocalVariable> body) { }

        public static void ARGUMENT(AbstractExpression value) { }

        public static AbstractExpression AWAIT(AbstractExpression promiseExpression) => null;
        public static FluentStatement DO => new FluentStatement();
        public static ThisExpression THIS => null;
        
        public static AbstractExpression DOT(this AbstractExpression target, AbstractMember member) => null;
        public static AbstractExpression DOT(this AbstractExpression target, string memberName) => null;
        public static AbstractExpression DOT(this LocalVariable target, AbstractMember member) => null;
        public static AbstractExpression DOT(this LocalVariable target, string memberName) => null;
        public static AbstractExpression DOT(this MethodParameter target, AbstractMember member) => null;

        public static AbstractExpression DOT(this MethodParameter target, string memberName)
            => new MemberExpression(target.Type, target.AsExpression(), MemberRef<AbstractMember>.Null, memberName);

        public static AbstractExpression NOT(AbstractExpression value)
            => new UnaryExpression(GetContextOrThrow().FindType<bool>(), UnaryOperator.LogicalNot, PopExpression(value));

        public static AbstractExpression NEW<T>(params object[] constructorArguments)
        {
            var context = GetContextOrThrow();
            return new NewObjectExpression(new MethodCallExpression(
                context.FindType<T>(),
                target: null,
                method: MemberRef<MethodMember>.Null,
                constructorArguments
                    .Select(value => new Argument(context.GetConstantExpression(value), MethodParameterModifier.None))
                    .ToImmutableList(),
                isAsyncAwait: false));
        }

        public static AbstractExpression NEW(TypeMember type, params object[] constructorArguments) => null;
        
        public static AbstractExpression ASSIGN(this AbstractExpression target, AbstractExpression value) 
            => PushExpression(new AssignmentExpression((IAssignable)PopExpression(target), PopExpression(value)));

        public static AbstractExpression ASSIGN(this MemberExpression member, AbstractExpression value) 
            => PushExpression(new AssignmentExpression(member, PopExpression(value)));

        public static AbstractExpression ASSIGN(this FieldMember target, AbstractExpression value)
            => PushExpression(new AssignmentExpression(target.AsThisMemberExpression(), PopExpression(value)));

        public static AbstractExpression INVOKE(this AbstractExpression target, params AbstractExpression[] arguments) => null;
        public static AbstractExpression INVOKE(this AbstractExpression target, Action body) => null;

        private static AbstractExpression PushExpression(AbstractExpression expression)
        {
            return BlockContext.Push(expression);
        }

        private static AbstractExpression PopExpression(AbstractExpression expression)
        {
            return BlockContext.Pop(expression);
        }
    }
}
