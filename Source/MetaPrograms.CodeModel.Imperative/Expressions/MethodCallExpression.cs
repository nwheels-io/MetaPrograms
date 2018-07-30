using System.Collections.Generic;
using System.Collections.Immutable;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class MethodCallExpression : InvocationExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitMethodCallExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }

            foreach (var argument in Arguments)
            {
                argument.Expression.AcceptVisitor(visitor);
            }
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteMethodCallExpression(this);
        }

        public AbstractExpression Target { get; set; }
        public MethodMemberBase Method { get; set; }
        public string MethodName { get; set; }
    }
}

