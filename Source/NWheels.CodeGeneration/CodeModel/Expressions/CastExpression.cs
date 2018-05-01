namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class CastExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitCastExpression(this);

            if (this.Type != null)
            {
                visitor.VisitReferenceToTypeMember(this.Type);
            }

            if (Source != null)
            {
                Source.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Source { get; set; }
    }
}
