using System;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MemberRefState
    {
        private AbstractMember _member;
        private bool _isMutable;

        public MemberRefState(AbstractMember initialMember)
        {
            _member = initialMember;
            _isMutable = true;
        }

        public T Get<T>()
            where T : AbstractMember
        {
            return (T)_member;
        }

        public MemberRefState MakeImmutable()
        {
            _isMutable = false;
            return this;
        }

        public void Reassign(AbstractMember enrichedMember)
        {
            if (!_isMutable)
            {
                throw new InvalidOperationException($"This member referecnce is immutable ({_member?.ToString() ?? "null"})");
            }

            _member = enrichedMember;
        }
    }
}