using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.Adapters.Reflection.Reader
{
    public class ClrTypeReader
    {
        public Type ClrType { get; } 
        public TypeMemberBuilder Builder { get; }
        public ClrTypeReaderResolver Resolver { get; }

        public ClrTypeReader(Type clrType, TypeMemberBuilder builder, ClrTypeReaderResolver resolver)
        {
            this.ClrType = clrType;
            this.Builder = builder;
            this.Resolver = resolver;
        }

        public void Read()
        {
            ReadName();
            ReadGenerics();
            ReadAncestors();
            ReadAttributes();
        }

        private void ReadName()
        {
            Builder.AssemblyName = ClrType.Assembly.GetName().Name;
            Builder.Namespace = ClrType.Namespace;
            Builder.Name = ClrType.Name;
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
            foreach (var clrAttribute in ClrType.GetCustomAttributes(inherit: true))
            {
                Builder.Attributes.Add(ReadAttribute(clrAttribute));
            }
        }

        private AttributeDescription ReadAttribute(object clrAttribute)
        {
            
        }
    }
}
