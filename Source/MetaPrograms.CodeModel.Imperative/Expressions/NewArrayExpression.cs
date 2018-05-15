using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class NewArrayExpression : AbstractExpression
    {
        public NewArrayExpression(
            MemberRef<TypeMember> type,
            MemberRef<TypeMember> elementType, 
            ImmutableList<AbstractExpression> dimensionLengths, 
            ImmutableList<ImmutableList<AbstractExpression>> dimensionInitializerValues) 
            : base(type)
        {
            ElementType = elementType;
            DimensionLengths = dimensionLengths;
            DimensionInitializerValues = dimensionInitializerValues;
        }

        public NewArrayExpression(
            NewArrayExpression source,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<MemberRef<TypeMember>>? elementType = null,
            Mutator<ImmutableList<AbstractExpression>>? dimensionLengths = null,
            Mutator<ImmutableList<ImmutableList<AbstractExpression>>>? dimensionInitializerValues = null) 
            : base(source, type)
        {
            ElementType = elementType.MutatedOrOriginal(source.ElementType);
            DimensionLengths = dimensionLengths.MutatedOrOriginal(source.DimensionLengths);
            DimensionInitializerValues = dimensionInitializerValues.MutatedOrOriginal(source.DimensionInitializerValues);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewArrayExpression(this);

            if (ElementType.Get() != null)
            {
                visitor.VisitReferenceToTypeMember(ElementType.Get());
            }

            if (DimensionLengths != null)
            {
                foreach (var length in DimensionLengths)
                {
                    length.AcceptVisitor(visitor);
                }
            }

            if (DimensionInitializerValues != null)
            {
                foreach (var valueList in DimensionInitializerValues)
                {
                    foreach (var value in valueList)
                    {
                        value.AcceptVisitor(visitor);
                    }
                }
            }
        }

        public AbstractExpression Length
        {
            get
            {
                if (DimensionLengths.Count == 0)
                {
                    throw new InvalidOperationException("Dimension lengths were not set");
                }

                if (DimensionLengths.Count != 1)
                {
                    throw new InvalidOperationException("This is a multi-dimensional array");
                }

                return DimensionLengths[0];
            }
            set
            {
                DimensionLengths.Clear();
                DimensionLengths.Add(value);
            }
        }

        public MemberRef<TypeMember> ElementType { get; }
        public ImmutableList<AbstractExpression> DimensionLengths { get; }
        public ImmutableList<ImmutableList<AbstractExpression>> DimensionInitializerValues { get; }
    }
}
