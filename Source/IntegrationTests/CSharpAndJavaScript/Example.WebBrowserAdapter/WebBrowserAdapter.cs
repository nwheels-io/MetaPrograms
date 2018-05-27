using System;
using System.Collections.Immutable;
using System.IO;
using MetaPrograms.CodeModel.Imperative;

namespace Example.WebBrowserAdapter
{
    public class WebBrowserAdapter
    {
        private readonly ICodeGeneratorOutput _output;

        public WebBrowserAdapter(ICodeGeneratorOutput output)
        {
            _output = output;
        }
            
        public void GenerateImplementations(WebUIModel.Metadata.WebUIMetadata metadata)
        { 
        }
    }
}
