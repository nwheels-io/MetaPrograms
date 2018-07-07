﻿using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class EnumMember : AbstractMember
    {
        public override void AcceptVisitor(MemberVisitor visitor)
        {
            base.AcceptVisitor(visitor);
            visitor.VisitEnumMember(this);
        }

        public object Value { get; set; }
    }
}
