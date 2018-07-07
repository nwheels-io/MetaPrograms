using System;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public static class FluentHelpers
    {
        public static TypeMember BuildTypeMember(TypeMemberKind typeKind, string name, Action body)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var traits = context.PopStateOrThrow<MemberTraitsContext>();
            var namespaceContext = context.TryLookupState<NamespaceContext>();
            var moduleContext = context.TryLookupState<ModuleMemberContext>();
            var containingType = context.TryLookupState<TypeMember>();

            var type = new TypeMember();
            type.Namespace = namespaceContext?.Name;
            type.Name = name;
            type.TypeKind = typeKind;
            type.DeclaringType = containingType;
            type.Modifier = traits.Modifier;
            type.Visibility = traits.Visibility;

            context.AddGeneratedMember(type, isTopLevel: containingType == null);

            using (context.PushState(type))
            {
                body?.Invoke();
            }

            return type;
        }

        public static AttributeDescription BuildAttribute(CodeGeneratorContext context, TypeMember type, object[] constructorArgumentsAndBody)
        {
            var body = constructorArgumentsAndBody.OfType<Action>().FirstOrDefault();
            var constructorArguments = constructorArgumentsAndBody
                .Where(x => !(x is Action))
                .Select(context.GetConstantExpression);

            var attribute = new AttributeDescription {
                AttributeType = type,
                ConstructorArguments = constructorArguments.ToList() 
            };

            var attributeContext = new AttributeContext(attribute);

            if (body != null)
            {
                using (context.PushState(attributeContext))
                {
                    body();
                }
            }

            return attribute;
        }

        // public static TMember BuildMethodMember<TMember>(string name, TypeMember returnType, Action body)
        // {
        //     
        // }

        //public static void BuildMemberAttribute(TypeMember type, object[] constructorArgumentsAndBody)
        //{
        //    var context = GetContextOrThrow();
        //    var member = context.GetCurrentMember();
        //    var attributeContext = new AttributeContext();

        //    var body = constructorArgumentsAndBody.OfType<Action>().FirstOrDefault();
        //    var constructorArguments = constructorArgumentsAndBody
        //        .Where(x => !(x is Action))
        //        .Select(context.GetConstantExpression);

        //    if (body != null)
        //    {
        //        using (context.PushState(attributeContext))
        //        {
        //            body();
        //        }
        //    }

        //    var newAttribute = new AttributeDescription(
        //        type.GetRef(),
        //        constructorArguments.ToImmutableList(),
        //        attributeContext.NamedProperties.ToImmutableList());

        //    member.WithAttributes(member.Attributes.Add(newAttribute), shouldReplaceSource: true);
        //}
    }
}
