using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public struct MemberRef<TMember> : IEquatable<MemberRef<TMember>>, IMemberRef
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

        AbstractMember IMemberRef.Get()
        {
            return Get();
        }

        Type IMemberRef.MemberType => typeof(TMember);

        public TMember Get()
        {
            return _state?.Get<TMember>();
        }

        public MemberRef<TBaseMember> AsRef<TBaseMember>()
            where TBaseMember : AbstractMember
        {
            ValidateMemberIs<TBaseMember>();
            return new MemberRef<TBaseMember>(_state);
        }

        public override int GetHashCode()
        {
            return (_state?.GetHashCode()).GetValueOrDefault(0);
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

        public override string ToString()
        {
            return (Get()?.ToString() ?? "null");
        }

        public bool IsNull => Get() == null;
        public bool IsNotNull => Get() != null;

        private void ValidateMemberIs<TBaseMember>()
            where TBaseMember : AbstractMember
        {
            _state.Get<TBaseMember>();
        }

        public static readonly MemberRef<TMember> Null = 
            new MemberRef<TMember>(new MemberRefState(null).MakeImmutable());

        public static implicit operator TMember(MemberRef<TMember> @ref)
        {
            return @ref.Get();
        }
    }
}
