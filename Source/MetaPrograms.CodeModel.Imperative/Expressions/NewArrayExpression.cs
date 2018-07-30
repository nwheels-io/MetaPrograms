using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class NewArrayExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewArrayExpression(this);

            if (ElementType != null)
            {
                visitor.VisitReferenceToTypeMember(ElementType);
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

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteNewArrayExpression(this);
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

        public TypeMember ElementType { get; set; }
        public List<AbstractExpression> DimensionLengths { get; } = new List<AbstractExpression>();
        public List<List<AbstractExpression>> DimensionInitializerValues { get; set; } = new List<List<AbstractExpression>>();
    }
}
