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

        private readonly int _distance;

        public Type ClrType { get; } 
        public TypeMemberBuilder Builder { get; }
        public ClrTypeReaderResolver Resolver { get; }

        public ClrTypeReader(Type clrType, TypeMemberBuilder builder, ClrTypeReaderResolver resolver, int distance)
        {
            _distance = distance;
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
            Builder.TypeKind = ClrType.GetTypeMemberKind();
            Builder.Name = ClrType.Name;
            Builder.Namespace = ClrType.Namespace;
            Builder.AssemblyName = ClrType.Assembly.GetName().Name;
            Builder.Visibility = ClrType.GetMemberVisibility();
            Builder.Modifier = ClrType.GetMemberModifier();
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
                    Builder.GenericDefinition = Resolver.GetType(ClrType.GetGenericTypeDefinition(), _distance + 1);
                    Builder.GenericParameters.AddRange(Builder.GenericDefinition.Get().GenericParameters);
                    Builder.GenericArguments.AddRange(ClrType.GetGenericArguments().Select(Resolver.GetType));
                }
            }
        }

        private void ReadAncestors()
        {
            if (ClrType.BaseType != typeof(object))
            {
                Builder.BaseType = Resolver.GetType(ClrType.BaseType, _distance + 1);
            }

            foreach (var clrInterface in ClrType.GetInterfaces())
            {
                Builder.Interfaces.Add(Resolver.GetType(clrInterface, _distance + 1));
            }
        }

        private void ReadAttributes()
        {
            foreach (var clrAttribute in ClrType.GetCustomAttributesData())
            {
                Builder.Attributes.Add(ReadAttribute(clrAttribute));
            }
        }

        private ImmutableList<AttributeDescription> ReadAttributes(MemberInfo info)
        {
            var result = new List<AttributeDescription>();
            
            foreach (var clrAttribute in info.GetCustomAttributesData())
            {
                result.Add(ReadAttribute(clrAttribute));
            }

            return result.ToImmutableList();
        }

        private AttributeDescription ReadAttribute(CustomAttributeData clrAttribute)
        {
            var attributeType = Resolver.GetType(clrAttribute.AttributeType, _distance + 1); 
            
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

        private void ReadFields()
        {
            foreach (var info in ClrType.GetFields(LookupBindingFlags))
            {
                Builder.Members.Add(ReadField(info).GetRefAsAbstract());
            }
        }

        private FieldMember ReadField(FieldInfo info)
        {
            return new FieldMember(
                info.Name,
                Builder.GetTemporaryProxy().GetRef(),
                MemberStatus.Compiled,
                info.GetMemberVisibility(),
                info.GetMemberModifier(),
                ReadAttributes(info),
                Resolver.GetType(info.FieldType, _distance + 1),
                isReadOnly: false,
                initializer: null);
        }

        private void ReadConstructors()
        {
            foreach (var info in ClrType.GetConstructors(LookupBindingFlags))
            {
                //info.
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
