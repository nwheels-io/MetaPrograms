namespace NWheels.CodeGeneration.CodeModel.Members
{
    public class EnumMember : AbstractMember
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitEnumMember(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public object Value { get; set; }
    }
}
