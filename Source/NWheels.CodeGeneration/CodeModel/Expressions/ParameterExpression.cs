using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class ParameterExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitParameterExpression(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodParameter Parameter { get; set; }
    }
}
