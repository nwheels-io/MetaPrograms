using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative
{
    public readonly struct Mutator<T>
    {
        public Mutator(T value)
        {
            Value = value;
        }

        public readonly T Value;

        public static implicit operator Mutator<T>(T value)
        {
            return new Mutator<T>(value);
        }
    }

    public static class MutatorExtensions
    {
        public static T MutatedOrOriginal<T>(this Mutator<T>? mutator, T originalValue)
        {
            if (mutator.HasValue)
            {
                return mutator.Value.Value;
            }

            return originalValue;
        }
    }
}
