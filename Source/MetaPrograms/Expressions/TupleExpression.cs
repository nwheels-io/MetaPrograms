using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class TupleExpression : AbstractExpression
    {
        public TupleExpression(params LocalVariable[] variables)
        {
            Variables.AddRange(variables);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitTupleExpression(this);

            foreach (var variable in this.Variables)
            {
                variable.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteTupleExpression(this);
        }

        public List<LocalVariable> Variables { get; set; } = new List<LocalVariable>();
    }
}
