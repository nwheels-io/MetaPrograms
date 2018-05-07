using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Adapters.Roslyn.Tests
{
    public class HowRoslynWorks
    {
        [Test]
        public void CanCreateAdhocCompilation()
        {
            // arrange
            
            var sourceCode = "namespace MyTest { public class MyClass { public int MyMethod(string s) => 0; } }";
            TestWorkspaceFactory.CreatreDefaultWithCode(sourceCode, out Project project);

            // act 
            
            var compilation = project.GetCompilationAsync().Result;
            
            // assert
            
            compilation.ShouldNotBeNull();

            foreach (var diag in compilation.GetDiagnostics())
            {
                Console.WriteLine(diag.ToString());
            }
            
            compilation.GetDiagnostics().Count(d => d.Severity >= DiagnosticSeverity.Warning).ShouldBe(0);

            var myClassSymbol = compilation.GetTypeByMetadataName("MyTest.MyClass");
            myClassSymbol.ShouldNotBeNull();
            
            var myMethodSymbols = myClassSymbol.GetMembers("MyMethod");
            myMethodSymbols.Length.ShouldBe(1);
        }
    }
}