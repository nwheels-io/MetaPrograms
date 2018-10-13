using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using static MetaPrograms.CodeGeneratorContext;

namespace MetaPrograms.Fluent
{
    public static class FluentHelpers
    {
        public static TypeMember BuildTypeMember(TypeMemberKind typeKind, IdentifierName name, Action body)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var module = context.TryLookupState<ModuleMember>();
            var namespaceContext = context.TryLookupState<NamespaceContext>();
            var traits = context.PopStateOrThrow<MemberTraitsContext>();
            var containingType = context.TryLookupState<TypeMember>();

            var type = new TypeMember();
            type.Name = name;
            type.Namespace = namespaceContext?.Name;
            type.TypeKind = typeKind;
            type.DeclaringModule = module;
            type.DeclaringType = containingType;
            type.Modifier = traits.Modifier;
            type.Visibility = traits.Visibility;
            type.IsDefaultExport = traits.IsDefaultExport;

            if (containingType != null)
            {
                containingType.Members.Add(type);
            }

            context.AddGeneratedMember(type, isTopLevel: containingType == null);
            if (module != null)
            {
                module.Members.Add(type);
            }

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
