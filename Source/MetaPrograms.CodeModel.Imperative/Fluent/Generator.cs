using System;
using System.Collections.Generic;
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

        public static TypeMember MODULE(string name, Action body)
        {
            var visibilityContext = PUBLIC;
            var modifierContext = visibilityContext.STATIC;
            return FluentHelpers.BuildTypeMember(TypeMemberKind.Module, name, body);
        }

        public static IFluentImport IMPORT => new FluentImport();

        public static FluentVisibility PUBLIC => new FluentVisibility(MemberVisibility.Public);
        public static FluentVisibility EXPORT => new FluentVisibility(MemberVisibility.Public);
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

        public static void PARAMETER(string name, out MethodParameter @ref, Action body = null)
        {
            PARAMETER(MemberRef<TypeMember>.Null, name, out @ref, body);
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

        public static void LOCAL(string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            LOCAL(MemberRef<TypeMember>.Null, name, out @ref, initialValue);
        }

        public static void LOCAL(MemberRef<TypeMember> type, string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            @ref = new LocalVariable(name, type);
            var block = BlockContextBase.GetBlockOrThrow();
            block.AddLocal(@ref);
            block.AppendStatement(new VariableDeclarationStatement(@ref, initialValue));
        }

        public static void LOCAL<T>(string name, out LocalVariable @ref, AbstractExpression initialValue = null)
        {
            var type = GetContextOrThrow().FindType<T>();
            LOCAL(type, name, out @ref, initialValue);
        }

        public static void FINAL(string name, out LocalVariable @ref, AbstractExpression value)
        {
            FINAL(MemberRef<TypeMember>.Null, name, out @ref, value);
        }

        public static void FINAL(MemberRef<TypeMember> type, string name, out LocalVariable @ref, AbstractExpression value)
        {
            @ref = new LocalVariable(name, type, isFinal: true);
            var block = BlockContextBase.GetBlockOrThrow();
            block.AddLocal(@ref);
            block.AppendStatement(new VariableDeclarationStatement(@ref, initialValue: value));
        }

        public static void FINAL<T>(string name, out LocalVariable @ref, AbstractExpression value)
        {
            var type = GetContextOrThrow().FindType<T>();
            FINAL(type, name, out @ref, value);
        }

        public static TupleExpression TUPLE(string name1, out LocalVariable var1)
        {
            var tuple = MakeTuple(new[] { name1 }, types: null, out var variables);
            var1 = variables[0];
            return tuple;
        }

        public static TupleExpression TUPLE(string name1, MemberRef<TypeMember> type1, out LocalVariable var1)
        {
            var tuple = MakeTuple(new[] { name1 }, types: new[] { type1 }, out var variables);
            var1 = variables[0];
            return tuple;
        }

        public static void GET() => throw new NotImplementedException();
        public static void GET(Action body) => throw new NotImplementedException();
        public static void SET(Action<LocalVariable> body) => throw new NotImplementedException();

        public static void ARGUMENT(AbstractExpression value) 
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value);

        public static void ARGUMENT_BYREF(AbstractExpression value)
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value, MethodParameterModifier.Ref);

        public static void ARGUMENT_OUT(AbstractExpression value)
            => GetContextOrThrow().PeekStateOrThrow<InvocationContext>().AddArgument(value, MethodParameterModifier.Out);

        public static AbstractExpression AWAIT(AbstractExpression promiseExpression)
            => new AwaitExpression(promiseExpression.Type, promiseExpression);

        public static FluentStatement DO 
            => new FluentStatement();

        public static ThisExpression THIS 
            => new ThisExpression(GetContextOrThrow().GetCurrentTypeBuilder().GetRef());

        public static AbstractExpression DOT(this AbstractExpression target, AbstractMember member)
            => new MemberExpression(MemberRef<TypeMember>.Null, target, member.GetAbstractRef());

        public static AbstractExpression DOT(this AbstractExpression target, string memberName)
            => new MemberExpression(MemberRef<TypeMember>.Null, target, MemberRef<AbstractMember>.Null, memberName);

        public static AbstractExpression DOT(this LocalVariable target, AbstractMember member) => throw new NotImplementedException();
        public static AbstractExpression DOT(this LocalVariable target, string memberName) => throw new NotImplementedException();
        public static AbstractExpression DOT(this MethodParameter target, AbstractMember member) => throw new NotImplementedException();

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
                method: MemberRef<MethodMemberBase>.Null,
                constructorArguments
                    .Select(value => new Argument(context.GetConstantExpression(value), MethodParameterModifier.None))
                    .ToImmutableList()));
        }

        public static AbstractExpression NEW(TypeMember type, params object[] constructorArguments) => throw new NotImplementedException();

        public static AbstractExpression INITOBJECT(params (string key, AbstractExpression value)[] initializers)
        {
            return new ObjectInitializerExpression(initializers.Select(init => new NamedPropertyValue(init.key, init.value)));
        }

        public static AbstractExpression INITOBJECT(Action body)
        {
            var initializerContext = new ObjectInitializerContext();

            using (GetContextOrThrow().PushState(initializerContext))
            {
                body?.Invoke();
            }

            return new ObjectInitializerExpression(initializerContext.PropertyValues);
        }

        public static AbstractExpression ASSIGN(this AbstractExpression target, AbstractExpression value) 
            => PushExpression(new AssignmentExpression((IAssignable)PopExpression(target), PopExpression(value)));

        public static AbstractExpression ASSIGN(this MemberExpression member, AbstractExpression value) 
            => PushExpression(new AssignmentExpression(member, PopExpression(value)));

        public static AbstractExpression ASSIGN(this FieldMember target, AbstractExpression value)
            => PushExpression(new AssignmentExpression(target.AsThisMemberExpression(), PopExpression(value)));

        public static AbstractExpression ASSIGN(this LocalVariable target, AbstractExpression value)
            => PushExpression(new AssignmentExpression(target, PopExpression(value)));

        public static AbstractExpression INVOKE(this AbstractExpression expression, params AbstractExpression[] arguments)
            => INVOKE(expression, arguments.Select(arg => new Argument(arg, MethodParameterModifier.None)));

        public static AbstractExpression INVOKE(this AbstractExpression expression, IEnumerable<Argument> arguments)
        {
            if (expression is MemberExpression memberExpression)
            {
                var target = memberExpression.Target;
                var member = memberExpression.Member.AsRef<MethodMemberBase>();
                var returnType = (member.IsNotNull ? member.Get().ReturnType : MemberRef<TypeMember>.Null);
                return new MethodCallExpression(returnType, target, member, arguments.ToImmutableList(), memberExpression.MemberName);
            }

            return new DelegateInvocationExpression(expression.Type, arguments.ToImmutableList(), expression);
        }

        public static AbstractExpression INVOKE(this AbstractExpression expression, Action body)
        {
            var invocation = new InvocationContext();

            using (GetContextOrThrow().PushState(invocation))
            {
                body?.Invoke();
            }

            return INVOKE(expression, invocation.GetArguments());
        }

        //public static AnonymousDelegateExpression LAMBDA()

        public static AbstractExpression TYPED(object value)
        {
            var context = GetContextOrThrow();

            return AbstractExpression.FromValue(value, resolveType: type => {
                if (context.TryFindMember<TypeMember>(type, out var typeRef))
                {
                    return typeRef;
                }
                return MemberRef<TypeMember>.Null;
            });
        }

        public static AbstractExpression ANY(object value)
        {
            return AbstractExpression.FromValue(value, resolveType: t => MemberRef<TypeMember>.Null);
        }

        private static AbstractExpression PushExpression(AbstractExpression expression)
        {
            return BlockContextBase.Push(expression);
        }

        private static AbstractExpression PopExpression(AbstractExpression expression)
        {
            return BlockContextBase.Pop(expression);
        }

        private static TupleExpression MakeTuple(string[] names, MemberRef<TypeMember>[] types, out LocalVariable[] variables)
        {
            variables = names.Select((name, index) => new LocalVariable(
                name,
                types?[index] ?? MemberRef<TypeMember>.Null
            )).ToArray();

            return new TupleExpression(variables);
        }
    }
}
