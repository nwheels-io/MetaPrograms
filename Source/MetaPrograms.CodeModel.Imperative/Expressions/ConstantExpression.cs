using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative.Expressions
{
    public class ConstantExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitConstantExpression(this);

            if (Value is TypeMember typeMember)
            {
                visitor.VisitReferenceToTypeMember(typeMember);
            }

            //TODO: handle bindings
            //else if (Value is System.Type systemType)
            //{
            //    visitor.VisitReferenceToTypeMember(systemType);
            //}
            //else if (Value != null)
            //{
            //    visitor.VisitReferenceToTypeMember(Value.GetType());
            //}
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            return rewriter.RewriteConstantExpression(this);
        }

        public object Value { get; set; }

        public static ConstantExpression Null => new ConstantExpression {
            Type = null,
            Value = null
        };
    }
}
