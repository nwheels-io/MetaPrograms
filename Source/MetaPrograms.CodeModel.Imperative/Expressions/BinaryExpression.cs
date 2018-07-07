﻿using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class BinaryExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitBinaryExpression(this);

            if (Left != null)
            {
                Left.AcceptVisitor(visitor);
            }

            if (Right != null)
            {
                Right.AcceptVisitor(visitor);
            }
        }

        public AbstractExpression Left { get; set; }
        public BinaryOperator Operator { get; set; }
        public AbstractExpression Right { get; set; }
    }
}
