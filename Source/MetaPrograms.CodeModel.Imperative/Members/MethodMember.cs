using System.Reflection;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodMember : MethodMemberBase
    {
        public MethodMember(
            MemberVisibility visibility, 
            string name, 
            params MethodParameter[] parameters)
            : this(visibility, MemberModifier.None, name, new MethodSignature() {  })
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodMember(
            MemberVisibility visibility,
            string name,
            MethodSignature signature)
            : this(visibility, MemberModifier.None, name, signature)
        {
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodMember(
            MemberVisibility visibility,
            MemberModifier modifier,
            string name,
            MethodSignature signature)
        {
            this.Visibility = visibility;
            this.Modifier = modifier;
            this.Name = name;
            this.Signature = signature;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodMember(MethodInfo clrBinding)
            : base(clrBinding)
        {
            this.Name = Name;
            this.Signature = new MethodSignature(clrBinding);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitMethod(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodInfo ClrBinding { get; set; }
    }
}
