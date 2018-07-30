using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using MetaPrograms.CodeModel.Imperative.Members;

namespace MetaPrograms.CodeModel.Imperative
{
    public class ImperativeCodeModel
    {
        private readonly HashSet<AbstractMember> _topLevelMembers;
        private readonly Dictionary<object, AbstractMember> _membersByBndings;

        public ImperativeCodeModel()
            : this(topLevelMembers: new AbstractMember[0], membersByBindings: null)
        {
        }

        public ImperativeCodeModel(
            IEnumerable<AbstractMember> topLevelMembers, 
            IDictionary<object, AbstractMember> membersByBindings = null)
        {
            _topLevelMembers = new HashSet<AbstractMember>(topLevelMembers);
            _membersByBndings = (
                membersByBindings?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ??
                new Dictionary<object, AbstractMember>());
        }

        public IEnumerable<AbstractMember> TopLevelMembers => _topLevelMembers;
        public IReadOnlyDictionary<object, AbstractMember> MembersByBndings => _membersByBndings;

        public TMember Get<TMember>(object binding)
            where TMember : AbstractMember
        {
            return (TMember)MembersByBndings[binding];
        }

        public TMember TryGet<TMember>(object binding)
            where TMember : AbstractMember
        {
            if (_membersByBndings.TryGetValue(binding, out var member))
            {
                return (TMember)member;
            }

            return null;
        }

        public void Add<TMember>(TMember member, bool isTopLevel = false)
            where TMember : AbstractMember
        {
            foreach (var binding in member.Bindings)
            {
                if (_membersByBndings.TryGetValue(binding, out var existingMember))
                {
                    if (!ReferenceEquals(member, existingMember))
                    {
                        throw new ArgumentException($"Member already registered: '{member.ToString()}'.");                        
                    }
                }
                else
                {
                    _membersByBndings.Add(binding, member);
                }
            }

            if (isTopLevel)
            {
                _topLevelMembers.Add(member);
            }
        }
    }
}
