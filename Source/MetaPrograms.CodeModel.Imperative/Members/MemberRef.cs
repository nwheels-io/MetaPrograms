using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public struct MemberRef<TMember> : IEquatable<MemberRef<TMember>>
        where TMember : AbstractMember
    {
        private readonly MemberRefState _state;

        internal MemberRef(TMember member)
        {
            _state = new MemberRefState(member);
        }

        public MemberRef(MemberRefState state)
        {
            _state = state;
        }

        public TMember Get()
        {
            return _state?.Get<TMember>();
        }

        public MemberRef<TBaseMember> AsRef<TBaseMember>()
            where TBaseMember : AbstractMember
        {
            return new MemberRef<TBaseMember>(_state);
        }

        public override int GetHashCode()
        {
            return _state.GetHashCode();
        }

        public bool Equals(MemberRef<TMember> other)
        {
            return (this._state == other._state);
        }

        public override bool Equals(object obj)
        {
            if (obj is MemberRef<TMember> typedRef)
            {
                return this.Equals(typedRef);
            }
            return false;
        }

        public bool IsNull => Get() == null;
        public bool IsNotNull => Get() != null;

        public static readonly MemberRef<TMember> Null = 
            new MemberRef<TMember>(new MemberRefState(null).MakeImmutable());

        public static implicit operator TMember(MemberRef<TMember> @ref)
        {
            return @ref.Get();
        }
    }

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
            _isMutable = true;
            return this;
        }

        public void ReassignOnce(AbstractMember finalMember)
        {
            if (!_isMutable)
            {
                throw new InvalidOperationException($"This member referecnce is immutable ({_member?.Name ?? "null"})");
            }

            _isMutable = false;
            _member = finalMember;
        }
    }
}
