using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class ModuleMember : AbstractMember
    {
        public ModuleMember(string name, IEnumerable<MemberRef<AbstractMember>> members)
            : this(name, MemberStatus.Generator, members.ToImmutableList(), selfReference: null)
        {
        }

        public ModuleMember(
            string name, 
            MemberStatus status,
            ImmutableList<MemberRef<AbstractMember>> members,
            MemberRefState selfReference = null)
            : base(
                name, 
                declaringType: MemberRef<TypeMember>.Null, 
                status, 
                visibility: MemberVisibility.Public, 
                modifier: MemberModifier.None, 
                attributes: ImmutableList<AttributeDescription>.Empty,
                selfReference)
        {
            this.Members = members;
        }

        public ModuleMember(
            AbstractMember source, 
            Mutator<string>? name = null, 
            Mutator<MemberStatus>? status = null, 
            Mutator<ImmutableList<AttributeDescription>>? attributes = null, 
            bool shouldReplaceSource = false)
            : base(
                source, 
                name, 
                declaringType: null, 
                status, 
                visibility: null, 
                modifier: null, 
                attributes, 
                shouldReplaceSource)
        {
        }

        public override AbstractMember WithAttributes(
            ImmutableList<AttributeDescription> attributes, bool shouldReplaceSource = false)
        {
            throw new NotSupportedException("Modules don't support attributes");
        }

        public ImmutableList<MemberRef<AbstractMember>> Members { get; }

        public MemberRef<ModuleMember> GetRef() => new MemberRef<ModuleMember>(SelfReference);
    }
}
