using System;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;
using static MetaPrograms.CodeModel.Imperative.CodeGeneratorContext;

namespace MetaPrograms.CodeModel.Imperative.Fluent
{
    public static  class FluentHelpers
    {
        public static TypeMember BuildTypeMember(TypeMemberKind typeKind, string name, Action body)
        {
            var context = CodeGeneratorContext.GetContextOrThrow();
            var traits = context.PopStateOrThrow<MemberTraitsContext>();
            var namespaceContext = context.TryLookupState<NamespaceContext>();
            var containingTypeRef = context.TryLookupState<MemberRef<TypeMember>>();

            var builder = new TypeMemberBuilder();
            builder.Namespace = namespaceContext?.Name;
            builder.Name = name;
            builder.TypeKind = typeKind;
            builder.DeclaringType = containingTypeRef;
            builder.Modifier = traits.Modifier;
            builder.Visibility = traits.Visibility;

            context.AddGeneratedMember(builder.GetTemporaryProxy().GetRef(), isTopLevel: containingTypeRef.IsNull);

            using (context.PushState(builder))
            {
                body?.Invoke();
            }

            var finalMember = new RealTypeMember(builder);
            builder.GetMemberSelfReference().Reassign(finalMember);
            return finalMember;
        }

        public static AttributeDescription BuildAttribute(CodeGeneratorContext context, TypeMember type, object[] constructorArgumentsAndBody)
        {
            var attributeContext = new AttributeContext();

            var body = constructorArgumentsAndBody.OfType<Action>().FirstOrDefault();
            var constructorArguments = constructorArgumentsAndBody
                .Where(x => !(x is Action))
                .Select(context.GetConstantExpression);

            if (body != null)
            {
                using (context.PushState(attributeContext))
                {
                    body();
                }
            }

            var newAttribute = new AttributeDescription(
                type.GetRef(),
                constructorArguments.ToImmutableList(),
                attributeContext.NamedProperties.ToImmutableList());

            return newAttribute;
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
