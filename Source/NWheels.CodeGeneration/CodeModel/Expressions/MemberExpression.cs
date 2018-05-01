using NWheels.CodeGeneration.CodeModel.Members;

namespace NWheels.CodeGeneration.CodeModel.Expressions
{
    public class MemberExpression : AbstractExpression
    {
        public override void AcceptVisitor(StatementVisitor visitor)
        {
            visitor.VisitMemberExpression(this);

            if (Target != null)
            {
                Target.AcceptVisitor(visitor);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public AbstractExpression Target { get; set; }
        public AbstractMember Member { get; set; }
        public string MemberName { get; set; }
    }
}
