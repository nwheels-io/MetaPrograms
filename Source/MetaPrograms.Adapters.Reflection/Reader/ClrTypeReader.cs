using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReader
    {
        private const BindingFlags LookupBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        
        public Type ClrType { get; } 
        public TypeMemberBuilder Builder { get; }
        public ClrTypeReaderResolver Resolver { get; }

        public ClrTypeReader(Type clrType, TypeMemberBuilder builder, ClrTypeReaderResolver resolver)
        {
            this.ClrType = clrType;
            this.Builder = builder;
            this.Resolver = resolver;
        }

        public void ReadNameOnly()
        {
            ReadName();
        }

        public void ReadAll()
        {
            ReadName();

            if (Builder.TypeKind != TypeMemberKind.GenericParameter)
            {
                ReadGenerics();
                ReadAncestors();
                ReadAttributes();

                if (Builder.TypeKind != TypeMemberKind.Primitive)
                {
                    ReadMembers();
                }
            }

            Builder.Status = MemberStatus.Compiled;
        }

        private void ReadName()
        {
            Builder.TypeKind = ReadTypeKind();
            Builder.Name = ClrType.Name;
            Builder.Namespace = ClrType.Namespace;
            Builder.AssemblyName = ClrType.Assembly.GetName().Name;
            Builder.Visibility = ReadVisibility();
            Builder.Modifier = ReadModifier();
        }

        private void ReadGenerics()
        {
            Builder.IsGenericType = ClrType.IsGenericType;
            Builder.IsGenericDefinition = ClrType.IsGenericTypeDefinition;
            Builder.IsGenericParameter = ClrType.IsGenericParameter;

            if (ClrType.IsGenericType)
            {
                if (ClrType.IsGenericTypeDefinition)
                {
                    Builder.GenericParameters.AddRange(ClrType.GetGenericArguments().Select(Resolver.GetType));
                }
                else
                {
                    Builder.GenericDefinition = Resolver.GetType(ClrType.GetGenericTypeDefinition());
                    Builder.GenericParameters.AddRange(Builder.GenericDefinition.Get().GenericParameters);
                    Builder.GenericArguments.AddRange(ClrType.GetGenericArguments().Select(Resolver.GetType));
                }
            }
        }

        private void ReadAncestors()
        {
            if (ClrType.BaseType != typeof(object))
            {
                Builder.BaseType = Resolver.GetType(ClrType.BaseType);
            }

            foreach (var clrInterface in ClrType.GetInterfaces())
            {
                Builder.Interfaces.Add(Resolver.GetType(clrInterface));
            }
        }

        private void ReadAttributes()
        {
            foreach (var clrAttribute in ClrType.GetCustomAttributesData())
            {
                Builder.Attributes.Add(ReadAttribute(clrAttribute));
            }
        }

        private AttributeDescription ReadAttribute(CustomAttributeData clrAttribute)
        {
            var attributeType = Resolver.GetType(clrAttribute.AttributeType); 
            
            var constructorArguments = clrAttribute.ConstructorArguments
                .Select(arg => Resolver.GetConstantExpression(arg.Value));

            var namedArguments = clrAttribute.NamedArguments
                .Select(arg => new NamedPropertyValue(
                    arg.MemberName, 
                    Resolver.GetConstantExpression(arg.TypedValue.Value)));

            return new AttributeDescription(
                attributeType,
                constructorArguments.ToImmutableList(),
                namedArguments.ToImmutableList());
        }

        private void ReadMembers()
        {
            ReadFields();
            ReadConstructors();
            ReadMethods();
            ReadProperties();
            ReadEvents();
        }

        private TypeMemberKind ReadTypeKind()
        {
            if (ClrType.IsClass)
            {
                return TypeMemberKind.Class;
            }
            
            if (ClrType.IsEnum)
            {
                return TypeMemberKind.Enum;
            }

            if (ClrType.IsValueType)
            {
                return (ClrType.IsPrimitive ? TypeMemberKind.Primitive : TypeMemberKind.Struct);
            }

            if (ClrType.IsInterface)
            {
                return TypeMemberKind.Enum;
            }

            if (ClrType.IsGenericParameter)
            {
                return TypeMemberKind.GenericParameter;
            }
            
            throw new NotSupportedException($"CLR type '{ClrType.FullName}' is not of a supported kind.");
        }

        private MemberVisibility ReadVisibility()
        {
            if (ClrType.IsNestedPrivate)
            {
                return MemberVisibility.Private;
            }

            if (ClrType.IsNestedAssembly)
            {
                return MemberVisibility.Internal;
            }

            if (ClrType.IsNestedFamORAssem)
            {
                return MemberVisibility.InternalProtected;
            }

            if (ClrType.IsNestedFamANDAssem)
            {
                return MemberVisibility.PrivateProtected;
            }

            return (
                ClrType.IsPublic || ClrType.IsNestedPublic 
                ? MemberVisibility.Public 
                : MemberVisibility.Internal);
        }

        private MemberModifier ReadModifier()
        {
            return (ClrType.IsAbstract ? MemberModifier.Abstract : MemberModifier.None);
        }

        private void ReadFields()
        {
            foreach (var info in ClrType.GetFields(LookupBindingFlags))
            {
                
            }
        }

        private void ReadConstructors()
        {
            foreach (var info in ClrType.GetConstructors(LookupBindingFlags))
            {
                
            }
        }

        private void ReadMethods()
        {
            foreach (var info in ClrType.GetMethods(LookupBindingFlags))
            {
                
            }
        }

        private void ReadProperties()
        {
            foreach (var info in ClrType.GetProperties(LookupBindingFlags))
            {
                
            }
        }

        private void ReadEvents()
        {
            foreach (var info in ClrType.GetEvents(LookupBindingFlags))
            {
                
            }
        }
    }
}
