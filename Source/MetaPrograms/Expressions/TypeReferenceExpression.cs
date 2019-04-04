using MetaPrograms.Members;

namespace MetaPrograms.Expressions
{
    public class TypeReferenceExpression : AbstractExpression
    {
        public TypeReferenceExpression(TypeMember operand)
        {
            this.TypeOperand = operand;
        }

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitReferenceToTypeMember(TypeOperand);
        }

        public override AbstractExpression AcceptRewriter(StatementRewriter rewriter)
        {
            var newType = rewriter.RewriteReferenceToTypeMember(TypeOperand);
            return (newType == TypeOperand ? this : new TypeReferenceExpression(newType));
        }
        
        public TypeMember TypeOperand { get; set; }
    }
}
