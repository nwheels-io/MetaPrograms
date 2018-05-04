using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaPrograms.CodeModel.Imperative
{
    public class BindingCollection : IEnumerable<object>
    {
        private readonly HashSet<object> _bindings = new HashSet<object>();

        public IEnumerator<object> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        public void Add<TBinding>(TBinding binding) where TBinding : class
        {
            _bindings.Add(binding);
        }

        public int Count => _bindings.Count;
    }
}
