using System;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public interface IMemberRef
    {
        AbstractMember Get();

        MemberRef<TBaseMember> AsRef<TBaseMember>()
            where TBaseMember : AbstractMember;
        
        Type MemberType { get; }
    }
}