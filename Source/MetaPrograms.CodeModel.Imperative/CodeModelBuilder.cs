using System;
using System.Collections.Generic;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class CodeModelBuilder
    {
        private readonly Dictionary<object, MemberRef<AbstractMember>> _memberByBinding = new Dictionary<object, MemberRef<AbstractMember>>();
        private readonly HashSet<MemberRef<AbstractMember>> _topLevelMembers = new HashSet<MemberRef<AbstractMember>>();
        
        //public void BindMember<TBinding>(MemberRef<AbstractMember> member, TBinding binding)
        //    where TBinding : class
        //{
        //    member.Bindings.Add(binding);
        //    _memberByBinding[binding] = member;
        //}

        public void RegisterMember(MemberRef<AbstractMember> member, bool isTopLevel)
        {
            foreach (var binding in member.Get().Bindings)
            {
                _memberByBinding[binding] = member;
            }

            if (isTopLevel)
            {
                _topLevelMembers.Add(member);
            }
        }

        //public bool TryGetMember<TMember, TBinding>(TBinding binding, out TMember member)
        //    where TMember : AbstractMember
        //    where TBinding : class
        //{
        //    if (_memberByBinding.TryGetValue(binding, out AbstractMember abstractMember))
        //    {
        //        member = (TMember)abstractMember;
        //        return true;
        //    }

        //    member = default;
        //    return false;
        //}

        public MemberRef<TMember> GetMember<TMember, TBinding>(TBinding binding)
            where TMember : AbstractMember
            where TBinding : class
        {
            if (_memberByBinding.TryGetValue(binding, out MemberRef<AbstractMember> existingMember))
            {
                return existingMember.AsRef<TMember>();
            }

            throw new KeyNotFoundException(
                $"{typeof(TMember).Name} with binding '{typeof(TBinding).Name}={binding}' could not be found.");
        }

        public MemberRef<TMember> GetMember<TMember>(object binding)
            where TMember : AbstractMember
        {
            return GetMember<TMember, object>(binding);
        }

        //public TMember GetOrAddMember<TMember, TBinding>(TBinding binding, Func<TMember> memberFactory)
        //    where TMember : AbstractMember
        //    where TBinding : class
        //{
        //    if (_memberByBinding.TryGetValue(binding, out AbstractMember existingMember))
        //    {
        //        return (TMember)existingMember;
        //    }

        //    var newMember = memberFactory();
        //    BindMember(newMember, binding);

        //    return newMember;
        //}

        public IEnumerable<MemberRef<AbstractMember>> GetRgisteredMembers() => new HashSet<MemberRef<AbstractMember>>(_memberByBinding.Values);

        public IEnumerable<MemberRef<AbstractMember>> GetTopLevelMembers() => _topLevelMembers;
        
        public ImmutableCodeModel GetCodeModel()
        {
            return new ImmutableCodeModel(_topLevelMembers.Select(m => m.Get()));
        }
    }
}