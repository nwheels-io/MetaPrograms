namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class NewObjectExpression : AbstractExpression
    {
        public NewObjectExpression()
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitNewObjectExpression(this);

            if (ConstructorCall != null)
            {
                ConstructorCall.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodCallExpression ConstructorCall { get; set; }
    }
}
