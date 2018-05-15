using System.Collections.Generic;
using System.Collections.Immutable;

namespace MetaPrograms.CodeModel.Imperative.Members
{
    public class MethodParameter
    {
        public MethodParameter(
            string name, 
            int position,
            MemberRef<TypeMember> type, 
            MethodParameterModifier modifier, 
            ImmutableList<AttributeDescription> attributes)
        {
            Name = name;
            Position = position;
            Type = type;
            Modifier = modifier;
            Attributes = attributes;
        }

        public MethodParameter(
            MethodParameter source,
            Mutator<string>? name = null,
            Mutator<int>? position = null,
            Mutator<MemberRef<TypeMember>>? type = null,
            Mutator<MethodParameterModifier>? modifier = null,
            Mutator<ImmutableList<AttributeDescription>>? attributes = null)
        {
            Name = name.MutatedOrOriginal(source.Name);
            Position = position.MutatedOrOriginal(source.Position);
            Type = type.MutatedOrOriginal(source.Type);
            Modifier = modifier.MutatedOrOriginal(source.Modifier);
            Attributes = attributes.MutatedOrOriginal(source.Attributes);
        }

        public string Name { get; }
        public int Position { get; }
        public MemberRef<TypeMember> Type { get; }
        public MethodParameterModifier Modifier { get; }
        public ImmutableList<AttributeDescription> Attributes { get; }

        public static MethodParameter ReturnVoid => null;
    }
}
