using System;
using System.Collections.Generic;
using System.Text;
using MetaPrograms.CodeModel.Imperative.Members;

namespace Example.WebUIModel.Metadata
{
    public class EventMapMetadata
    {
        private readonly Dictionary<string, List<IFunctionContext>> _eventMap = new Dictionary<string, List<IFunctionContext>>();

        public void AddHandler(string eventName, IFunctionContext handler)
        {
            if (!_eventMap.TryGetValue(eventName, out var list))
            {
                list = new List<IFunctionContext>();
                _eventMap.Add(eventName, list);
            }

            list.Add(handler);
        }

        public IReadOnlyList<IFunctionContext> GetHandlers(string eventName)
        {
            if (_eventMap.TryGetValue(eventName, out var list))
            {
                return list;
            }

            return new IFunctionContext[0];
        }

        public IEnumerable<string> GetHandledEventNames() => _eventMap.Keys;
    }
}
