using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MetaPrograms.IntegrationTests.CSharpToJavaScript
{
    [TestFixture]
    public class CSharpToJavaScriptTests
    {
        [TestCaseSource(nameof(TestCaseList))]
        public void TranslateCSharpToJavaScript(TestCase testCase)
        {
            // arrange & act

            var adapter = new BrowserJavaScriptAdapter();
            adapter.AddCSharpSource(testCase.CSharp);

            var actualJavaScript = adapter.GenerateJavaScriptCode();

            // assert

            actualJavaScript.ShouldBeCodeEquivalentTo(testCase.ExpectedJavaScript);
        }

        public static readonly TestCase[] TestCaseList = {
            new TestCase(
                "hello world", 
                csharp: @"
                    using Browser = MetaPrograms.IntegrationTests.CSharpToJavaScript.BrowserProgrammingModel;
                    [Browser.Component]
                    public class Component1
                    {
                        public void Hello(string text)
                        {
                            Browser.Console.Log(""Hello"", text);
                        }
                    }
                ",
                expectedJavaScript: @"
                    class Component1 {
                        hello(text) {
                            console.log('Hello', text);
                        }
                    }
                "
            )
        };

        public class TestCase 
        {
            public TestCase(string description, string csharp, string expectedJavaScript)
            {
                Description = description;
                CSharp = csharp;
                ExpectedJavaScript = expectedJavaScript;
            }

            public string Description { get; }
            public string CSharp { get; }
            public string ExpectedJavaScript { get; }

            public override string ToString()
            {
                return Description;
            }
        }
    }
}
