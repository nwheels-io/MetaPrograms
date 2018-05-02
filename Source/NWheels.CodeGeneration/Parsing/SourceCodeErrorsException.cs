using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NWheels.CodeGeneration.Parsing
{
    public class SourceCodeErrorsException : Exception
    {
        public SourceCodeErrorsException(string message, IEnumerable<Diagnostic> diagnostics)
            : base(message)
        {
            this.Diagnostics = diagnostics.ToArray();
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public IReadOnlyList<Diagnostic> Diagnostics;

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        public string GetErrorsText()
        {
            return string.Join(
                Environment.NewLine,
                this.Diagnostics.Select(d => d.ToString()));
        }
    }
}
