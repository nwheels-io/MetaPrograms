using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public abstract class MemberRef<TMember> 
        where TMember : AbstractMember
    {
        public abstract TMember Get();

        public static implicit operator TMember(MemberRef<TMember> @ref)
        {
            return @ref?.Get();
        }
    }

    public class MutableMemberRef<TMember> : MemberRef<TMember>
        where TMember : AbstractMember
    {
        private TMember _member;
        private bool _reassignedOnce;

        public MutableMemberRef(TMember initialMember)
        {
            _member = initialMember;
            _reassignedOnce = false;
        }

        public override TMember Get()
        {
            return _member;
        }

        public void ReassignOnce(TMember finalMember)
        {
            if (_reassignedOnce)
            {
                throw new InvalidOperationException($"This member referecnce was already reassigned ({_member?.Name})");
            }

            _reassignedOnce = true;
            _member = finalMember;
        }
    }
}
