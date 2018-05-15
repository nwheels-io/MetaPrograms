using System;
using System.Collections.Generic;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeModelBuilder
    {
        private readonly Dictionary<object, AbstractMember> _memberByBinding = new Dictionary<object, AbstractMember>();
        private readonly HashSet<AbstractMember> _topLevelMembers = new HashSet<AbstractMember>();
        
        public void BindMember<TBinding>(AbstractMember member, TBinding binding)
            where TBinding : class
        {
            member.Bindings.Add(binding);
            _memberByBinding[binding] = member;
        }

        public void RegisterMember(AbstractMember member)
        {
            foreach (var binding in member.Bindings)
            {
                _memberByBinding[binding] = member;
            }
        }

        public void RegisterTopLevelMember(AbstractMember member)
        {
            _topLevelMembers.Add(member);
        }

        public bool TryGetMember<TMember, TBinding>(TBinding binding, out TMember member)
            where TMember : AbstractMember
            where TBinding : class
        {
            if (_memberByBinding.TryGetValue(binding, out AbstractMember abstractMember))
            {
                member = (TMember)abstractMember;
                return true;
            }

            member = default;
            return false;
        }

        public TMember GetMember<TMember, TBinding>(TBinding binding)
            where TMember : AbstractMember
            where TBinding : class
        {
            if (_memberByBinding.TryGetValue(binding, out AbstractMember existingMember))
            {
                return (TMember)existingMember;
            }

            throw new KeyNotFoundException(
                $"{typeof(TMember).Name} with binding '{typeof(TBinding).Name}={binding}' could not be found.");
        }

        public TMember GetOrAddMember<TMember, TBinding>(TBinding binding, Func<TMember> memberFactory)
            where TMember : AbstractMember
            where TBinding : class
        {
            if (_memberByBinding.TryGetValue(binding, out AbstractMember existingMember))
            {
                return (TMember)existingMember;
            }

            var newMember = memberFactory();
            BindMember(newMember, binding);

            return newMember;
        }

        public IEnumerable<AbstractMember> GetRgisteredMembers() => new HashSet<AbstractMember>(_memberByBinding.Values);

        public IEnumerable<AbstractMember> GetTopLevelMembers() => _topLevelMembers;
        
        public ImmutableCodeModel GetCodeModel()
        {
            return new ImmutableCodeModel(_topLevelMembers);
        }
    }
}