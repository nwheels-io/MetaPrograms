using System.Reflection;
using NWheels.CodeGeneration.CodeModel.Expressions;

namespace NWheels.CodeGeneration.CodeModel.Members
{
    public class ConstructorMember : MethodMemberBase
    {
        public ConstructorMember(
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

        public ConstructorMember(ConstructorInfo clrBinding)
            : base(clrBinding)
        {
            this.Name = Name;
            this.Signature = new MethodSignature(clrBinding);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitConstructor(this);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public MethodCallExpression CallThisConstructor { get; set; }
        public MethodCallExpression CallBaseConstructor { get; set; }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public ConstructorInfo ClrBinding { get; set; }
    }
}
