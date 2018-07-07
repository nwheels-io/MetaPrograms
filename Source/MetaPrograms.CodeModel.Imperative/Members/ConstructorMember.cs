using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Expressions;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ConstructorMember : MethodMemberBase
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitConstructor(this);
        }

        public MethodCallExpression CallThisConstructor { get; set; }
        public MethodCallExpression CallBaseConstructor { get; set; }
    }
}
