using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using MetaPrograms;
using MetaPrograms.Members;

namespace Example.WebUIModel.Metadata
{
    public class EventMapMetadata
    {
        private readonly Dictionary<IdentifierName, List<IFunctionContext>> _eventMap = new Dictionary<IdentifierName, List<IFunctionContext>>();

        public void AddHandler(IdentifierName eventName, IFunctionContext handler)
        {
            if (!_eventMap.TryGetValue(eventName, out var list))
            {
                list = new List<IFunctionContext>();
                _eventMap.Add(eventName, list);
            }

            list.Add(handler);
        }

        public IReadOnlyList<IFunctionContext> GetHandlers(IdentifierName eventName)
        {
            if (_eventMap.TryGetValue(eventName, out var list))
            {
                return list;
            }

            return new IFunctionContext[0];
        }

        public IEnumerable<IdentifierName> GetHandledEventNames() => _eventMap.Keys;
    }
}
