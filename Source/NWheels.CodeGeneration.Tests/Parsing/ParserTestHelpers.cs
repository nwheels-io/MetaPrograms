using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NWheels.CodeGeneration.Tests.Parsing
{
    public static class ParserTestHelpers
    {
        public static MemoryStream MakeSourceStream(string code)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(code);
            writer.Flush();

            stream.Position = 0;
            return stream;
        }
    }
}
