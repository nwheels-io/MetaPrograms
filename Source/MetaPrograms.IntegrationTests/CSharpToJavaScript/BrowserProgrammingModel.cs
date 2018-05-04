using System;
using System.Collections.Generic;
using System.Text;

namespace MetaPrograms.IntegrationTests.CSharpToJavaScript
{
    public static class BrowserProgrammingModel
    {
        public class ComponentAttribute : Attribute
        {
        }

        public static class Console
        {
            public static void Log(params object[] values)
            {
                throw new NotImplementedException("This is a non-executable metadata member");
            }
        }
    }
}
