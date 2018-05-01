namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class AssignmentExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitAssignmentExpression(this);

            if (Left != null)
            {
                Left.AcceptVisitor(visitor);
            }

            if (Right != null)
            {
                Right.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Left { get; set; }
        public AbstractExpression Right { get; set; }
    }
}
