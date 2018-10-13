using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.Expressions;
using MetaPrograms.Statements;

namespace MetaPrograms.Members
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
