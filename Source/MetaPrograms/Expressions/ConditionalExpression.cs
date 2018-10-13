using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.Expressions
{
    public class ConditionalExpression : AbstractExpression
    {
        public AbstractExpression Condition { get; set; }
        public AbstractExpression WhenTrue { get; set; }
        public AbstractExpression WhenFalse { get; set; }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            Condition.AcceptVisitor(visitor);
            WhenTrue.AcceptVisitor(visitor);
            WhenFalse.AcceptVisitor(visitor);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            var newCondition = Condition.AcceptRewriter(rewriter);
            var newWhenTrue = WhenTrue.AcceptRewriter(rewriter);
            var newWhenFalse = WhenFalse.AcceptRewriter(rewriter);

            if (newCondition != Condition || newWhenTrue != WhenTrue || newWhenFalse != WhenFalse)
            {
                return new ConditionalExpression {
                    Bindings = new BindingCollection(this.Bindings),
                    Type = this.Type,
                    Condition = newCondition,
                    WhenTrue = newWhenTrue,
                    WhenFalse = newWhenFalse
                };
            }

            return this;
        }
    }
}
