﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetaPrograms
{
    public class BindingCollection : IEnumerable<object>
    {
        private readonly HashSet<object> _bindings = new HashSet<object>();

        public BindingCollection()
        {
        }

        public BindingCollection(BindingCollection source)
        {
            _bindings.UnionWith(source);
        }

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

        public void UnionWith<TBinding>(IEnumerable<TBinding> bindings) where TBinding : class
        {
            _bindings.UnionWith(bindings);
        }

        public T First<T>()
        {
            return _bindings.OfType<T>().First();
        }

        public T FirstOrDefault<T>()
        {
            return _bindings.OfType<T>().FirstOrDefault();
        }

        public int Count => _bindings.Count;
    }
}
