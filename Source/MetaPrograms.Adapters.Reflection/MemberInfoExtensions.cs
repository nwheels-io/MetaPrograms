using System;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection
{
    public static class MemberInfoExtensions
    {
        public static TypeMemberKind GetTypeMemberKind(this Type type)
        {
            if (type.IsClass)
            {
                return TypeMemberKind.Class;
            }
            
            if (type.IsEnum)
            {
                return TypeMemberKind.Enum;
            }

            if (type.IsValueType)
            {
                return (type.IsPrimitive ? TypeMemberKind.Primitive : TypeMemberKind.Struct);
            }

            if (type.IsInterface)
            {
                return TypeMemberKind.Enum;
            }

            if (type.IsGenericParameter)
            {
                return TypeMemberKind.GenericParameter;
            }
            
            throw new NotSupportedException($"CLR type '{type.FullName}' is not of a supported kind.");
        }
        
        public static MemberVisibility GetMemberVisibility(this Type type)
        {
            if (type.IsNestedPrivate)
            {
                return MemberVisibility.Private;
            }

            if (type.IsNestedAssembly)
            {
                return MemberVisibility.Internal;
            }

            if (type.IsNestedFamORAssem)
            {
                return MemberVisibility.InternalProtected;
            }

            if (type.IsNestedFamANDAssem)
            {
                return MemberVisibility.PrivateProtected;
            }

            return (
                type.IsPublic || type.IsNestedPublic 
                ? MemberVisibility.Public 
                : MemberVisibility.Internal);
        }

        public static MemberModifier GetMemberModifier(this Type type)
        {
            return (type.IsAbstract ? MemberModifier.Abstract : MemberModifier.None);
        }

        public static MemberVisibility GetMemberVisibility(this FieldInfo field)
        {
            if (field.IsPublic)
            {
                return MemberVisibility.Public;
            }

            if (field.IsPrivate)
            {
                return MemberVisibility.Private;
            }

            if (field.IsFamily)
            {
                return MemberVisibility.Protected;
            }

            if (field.IsFamilyOrAssembly)
            {
                return MemberVisibility.InternalProtected;
            }

            if (field.IsFamilyAndAssembly)
            {
                return MemberVisibility.PrivateProtected;
            }

            return MemberVisibility.Internal;
        }

        public static MemberModifier GetMemberModifier(this FieldInfo field)
        {
            return (field.IsStatic ? MemberModifier.Static : MemberModifier.None);
        }

        public static MemberVisibility GetMemberVisibility(this MethodBase method)
        {
            if (method.IsPublic)
            {
                return MemberVisibility.Public;
            }

            if (method.IsPrivate)
            {
                return MemberVisibility.Private;
            }

            if (method.IsFamily)
            {
                return MemberVisibility.Protected;
            }

            if (method.IsFamilyOrAssembly)
            {
                return MemberVisibility.InternalProtected;
            }

            if (method.IsFamilyAndAssembly)
            {
                return MemberVisibility.PrivateProtected;
            }

            return MemberVisibility.Internal;
        }

        public static MemberModifier GetMemberModifier(this MethodBase method)
        {
            if (method.IsStatic)
            {
                return MemberModifier.Static;
            }

            if (method.IsAbstract)
            {
                return MemberModifier.Abstract;
            }
            
            if (method.IsVirtual)
            {
                var isOverride = (method is MethodInfo info && info.GetBaseDefinition() != info);
                return (isOverride ? MemberModifier.Override : MemberModifier.Virtual);
            }

            return MemberModifier.None;
        }

        public static MethodParameterModifier GetMethodParameterModifier(this ParameterInfo parameter)
        {
            if (parameter.ParameterType.IsByRef)
            {
                return (parameter.IsOut ? MethodParameterModifier.Out : MethodParameterModifier.Ref);
            }

            return MethodParameterModifier.None;
        }
    }
}
