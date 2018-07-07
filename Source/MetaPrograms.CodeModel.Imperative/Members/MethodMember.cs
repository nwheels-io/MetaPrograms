using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using MetaPrograms.CodeModel.Imperative.Statements;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodMember : MethodMemberBase
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitMethod(this);
        }
    }
}
