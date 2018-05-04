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
        public static Mutator<T>? Assign<T>(this Mutator<T>? mutator, T newValue) 
            where T : class
        {
            if (newValue != null)
            {
                return newValue;
            }

            return default;
        }

        public static Mutator<T>? Assign<T>(this Mutator<T>? mutator, T? newValue) 
            where T : struct
        {
            if (newValue.HasValue)
            {
                return newValue.Value;
            }

            return default;
        }

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
