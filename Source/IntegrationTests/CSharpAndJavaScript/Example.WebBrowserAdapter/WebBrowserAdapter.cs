﻿using System;
using System.Collections.Immutable;
using System.IO;

namespace Example.WebBrowserAdapter
{
    public class WebBrowserAdapter
    {
        public WebBrowserAdapter(Func<string, Stream> outputStreamFactory)
        {
        }

        public ImmutableDictionary<string, Stream> GenerateImplementations(WebUIModel.WebUIModel model) =>
            ImmutableDictionary<string, Stream>.Empty;
    }
}