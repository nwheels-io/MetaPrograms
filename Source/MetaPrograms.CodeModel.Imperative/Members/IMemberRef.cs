using System;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public interface IMemberRef
    {
        AbstractMember Get();
        Type MemberType { get; }
    }
}