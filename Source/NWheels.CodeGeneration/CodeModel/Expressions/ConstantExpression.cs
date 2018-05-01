using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
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
            else if (Value is System.Type systemType)
            {
                visitor.VisitReferenceToTypeMember(systemType);
            }
            else if (Value != null)
            {
                visitor.VisitReferenceToTypeMember(Value.GetType());
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public object Value { get; set; }
    }
}
