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
}
