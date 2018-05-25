using System;
using System.Collections.Immutable;
using System.IO;
using Example.WebUIModel.Metadata;

namespace Example.AspNetAdapter
{
    public class AspNetAdapter
    {
        private readonly Func<string, Stream> _outputStreamFactory;

        public AspNetAdapter(Func<string, Stream> outputStreamFactory)
        {
            _outputStreamFactory = outputStreamFactory;
        }

        public ImmutableDictionary<string, Stream> GenerateImplementations(WebUIMetadata ui)
        {

        }
    }
}
