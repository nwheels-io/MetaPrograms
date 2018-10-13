using System;
using System.Collections.Generic;
using System.Text;

namespace Example.WebUIModel.Metadata
{
    public class MetadataExtensionMap
    {
        private readonly Dictionary<Type, object> _extensionDataByType = new Dictionary<Type, object>();

        public T Get<T>()
        {
            return (T)_extensionDataByType[typeof(T)];
        }

        public T TryGet<T>()
        {
            if (_extensionDataByType.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }

            return default;
        }

        public void Add<T>(T extension)
        {
            _extensionDataByType.Add(typeof(T), extension);
        }

        public void AddOrUpdate<T>(Func<T> factory, Action<T> init)
        {
            if (!_extensionDataByType.TryGetValue(typeof(T), out var value))
            {
                value = factory();
                _extensionDataByType.Add(typeof(T), value);
            }

            init((T)value);
        }

        public void AddOrUpdate<T>(Action<T> init)
            where T : class, new()
        {
            if (!_extensionDataByType.TryGetValue(typeof(T), out var value))
            {
                value = new T();
                _extensionDataByType.Add(typeof(T), value);
            }

            init((T)value);
        }
    }
}
