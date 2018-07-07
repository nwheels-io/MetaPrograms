using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
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

        public List<LocalVariable> Variables { get; set; } = new List<LocalVariable>();
    }
}
