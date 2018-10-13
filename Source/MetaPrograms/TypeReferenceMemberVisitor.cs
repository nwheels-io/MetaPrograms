using System.Collections.Generic;
using MetaPrograms.Members;

namespace MetaPrograms
{
    public class TypeReferenceMemberVisitor : MemberVisitor
    {
        private readonly HashSet<TypeMember> _referencedTypes;
        private readonly TypeReferenceStatementVisitor _statementVisitor;

        public TypeReferenceMemberVisitor(HashSet<TypeMember> referencedTypes)
        {
            _referencedTypes = referencedTypes;
            _statementVisitor = new TypeReferenceStatementVisitor(referencedTypes);
        }

        public override void VisitConstructor(ConstructorMember constructor)
        {
            base.VisitConstructor(constructor);

            if (constructor.CallThisConstructor != null)
            {
                constructor.CallThisConstructor.AcceptVisitor(_statementVisitor);
            }

            if (constructor.CallBaseConstructor != null)
            {
                constructor.CallBaseConstructor.AcceptVisitor(_statementVisitor);
            }
        }

        public override void VisitField(FieldMember field)
        {
            base.VisitField(field);
            AddReferencedType(field.Type);
        }

        public override void VisitProperty(PropertyMember property)
        {
            base.VisitProperty(property);
            AddReferencedType(property.PropertyType);
        }

        public override void VisitEvent(EventMember eventMember)
        {
            base.VisitEvent(eventMember);
            AddReferencedType(eventMember.DelegateType);
        }

        public override void VisitAttribute(AttributeDescription attribute)
        {
            base.VisitAttribute(attribute);
            AddReferencedType(attribute.AttributeType);
        }

        protected internal override void VisitTypeMember(TypeMember type)
        {
            base.VisitTypeMember(type);
            AddReferencedType(type);

            if (type.BaseType != null)
            {
                AddReferencedType(type.BaseType);
            }

            if (type.Interfaces != null)
            {
                foreach (var interfaceType in type.Interfaces)
                {
                    AddReferencedType(interfaceType);
                }
            }
        }

        protected internal override void VisitMethodBase(MethodMemberBase method)
        {
            base.VisitMethodBase(method);

            if (method.Signature != null)
            {
                AddReferencedType(method.Signature.ReturnValue?.Type);

                if (method.Signature.Parameters != null)
                {
                    foreach (var parameter in method.Signature.Parameters)
                    {
                        AddReferencedType(parameter.Type);
                    }
                }
            }

            if (method.Body != null)
            {
                method.Body.AcceptVisitor(_statementVisitor);
            }
        }

        private void AddReferencedType(TypeMember type)
        {
            AddReferencedType(_referencedTypes, type);
        }

        internal static void AddReferencedType(HashSet<TypeMember> typeSet, TypeMember type)
        {
            if (type != null)
            {
                if (type.IsArray)
                {
                    typeSet.Add(type.UnderlyingType);
                }
                else
                {
                    if (!type.IsGenericType || type.IsGenericDefinition)
                    {
                        typeSet.Add(type);
                    }

                    if (type.IsGenericType && !type.IsGenericDefinition)
                    {
                        AddReferencedType(typeSet, type.GenericTypeDefinition);

                        foreach (var argument in type.GenericArguments)
                        {
                            AddReferencedType(typeSet, argument);
                        }
                    }
                }
            }
        }
    }
}
