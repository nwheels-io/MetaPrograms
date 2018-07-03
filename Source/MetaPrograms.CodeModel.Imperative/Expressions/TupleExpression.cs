using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class TupleExpression : AbstractExpression
    {
        public TupleExpression(IEnumerable<LocalVariable> variables)
            : base(MemberRef<TypeMember>.Null)
        {
            this.Variables = variables.ToImmutableList();
        }

        public TupleExpression(
            TupleExpression source, 
            Mutator<ImmutableList<LocalVariable>>? variables = null)
            : base(source, type: null)
        {
            this.Variables = variables.MutatedOrOriginal(source.Variables);
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitTupleExpression(this);

            foreach (var variable in this.Variables)
            {
                variable.AcceptVisitor(visitor);
            }
        }

        public ImmutableList<LocalVariable> Variables { get; }
    }
}
