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

            // act 
            
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(sourceCode);
            
            // assert
            
            compilation.ShouldNotBeNull();
            compilation.GetDiagnostics().Count(d => d.Severity >= DiagnosticSeverity.Warning).ShouldBe(0);

            var myClassSymbol = compilation.GetTypeByMetadataName("MyTest.MyClass");
            myClassSymbol.ShouldNotBeNull();
            
            var myMethodSymbols = myClassSymbol.GetMembers("MyMethod");
            myMethodSymbols.Length.ShouldBe(1);
        }

        [Test]
        public void CanReferenceTypesFromThisAssembly()
        {
            // arrange
            
            var sourceCode = @"
                using MetaPrograms.Adapters.Roslyn.Tests;
                namespace MyTest { 
                    public class MyClass : HowRoslynWorks.ASimpleClass { 
                        public override void F() { 
                            base.F(); 
                        } 
                    }
                }
            ";

            // act 
            
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(sourceCode, references: new[] {
                this.GetType().Assembly.Location
            });
            
            // assert

            var myClassSymbol = compilation.GetTypeByMetadataName("MyTest.MyClass");
            
            var aSimpleClassSymbolFromBase = myClassSymbol.BaseType;
            aSimpleClassSymbolFromBase.ShouldNotBeNull();

            var aSimpleClassSymbolFromName = compilation.GetTypeByMetadataName(typeof(ASimpleClass).FullName);
            aSimpleClassSymbolFromName.ShouldBeSameAs(aSimpleClassSymbolFromBase);
        }

        [Test]
        public void CannotCompareSymbolsByReference()
        {
            //Arrange
            
            var sourceCode = @"
                using MetaPrograms.Adapters.Roslyn.Tests;
                namespace MyTest { 
                    public class MyClass1 : HowRoslynWorks.AGenericClass<int, string> { }
                    public class MyClass2 : HowRoslynWorks.AGenericClass<int, string> { }
                    public class MyClass3 { 
                        public HowRoslynWorks.AGenericClass<int, string> F;
                    }
                }
            ";

            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(sourceCode, references: ThisAssemblyLocation);

            //Act 

            var openType = compilation.GetTypeByMetadataName("MetaPrograms.Adapters.Roslyn.Tests.HowRoslynWorks+AGenericClass`2");

            var fromConstruct1 = openType.Construct(
                compilation.GetSpecialType(SpecialType.System_Int32),
                compilation.GetSpecialType(SpecialType.System_String));

            var fromConstruct2 = openType.Construct(
                compilation.GetSpecialType(SpecialType.System_Int32),
                compilation.GetSpecialType(SpecialType.System_String));

            var fromBase1 = compilation.GetTypeByMetadataName("MyTest.MyClass1").BaseType;
            var fromBase2 = compilation.GetTypeByMetadataName("MyTest.MyClass2").BaseType;
            var fromField = compilation.GetTypeByMetadataName("MyTest.MyClass3").GetMembers("F").OfType<IFieldSymbol>().First().Type;

            //Assert 

            fromConstruct1.ShouldNotBeNull();
            fromConstruct2.ShouldNotBeNull();
            fromBase1.ShouldNotBeNull();
            fromBase2.ShouldNotBeNull();
            fromField.ShouldNotBeNull();

            fromField.ShouldNotBeSameAs(fromBase1);
            fromBase2.ShouldNotBeSameAs(fromBase1);
            fromConstruct2.ShouldNotBeSameAs(fromConstruct1);
            fromConstruct2.ShouldNotBeSameAs(fromBase1);

            fromConstruct1.Equals(fromConstruct2).ShouldBeTrue();
            fromBase1.Equals(fromBase2).ShouldBeTrue();
            fromField.Equals(fromBase1).ShouldBeTrue();
            fromConstruct1.Equals(fromBase2).ShouldBeTrue();
        }

        [Test]
        public void CanUseSymbolsAsKeys()
        {
            //Arrange

            var sourceCode = @"
                using MetaPrograms.Adapters.Roslyn.Tests;
                namespace MyTest { 
                    public class MyClass1 : HowRoslynWorks.AGenericClass<int, string> { }
                }
            ";

            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(sourceCode, references: ThisAssemblyLocation);

            var closedTypeFromBase = compilation.GetTypeByMetadataName("MyTest.MyClass1").BaseType;

            var openType = compilation.GetTypeByMetadataName("MetaPrograms.Adapters.Roslyn.Tests.HowRoslynWorks+AGenericClass`2");
            var closedTypeFromConstruct = openType.Construct(
                compilation.GetSpecialType(SpecialType.System_Int32),
                compilation.GetSpecialType(SpecialType.System_String));

            var set = new HashSet<object>();
            set.Add(closedTypeFromConstruct);

            //Act 

            var matched = set.Contains(closedTypeFromBase);

            //Assert

            matched.ShouldBeTrue();
        }

        private static string[] ThisAssemblyLocation => new[] { typeof(HowRoslynWorks).Assembly.Location };

        public class ASimpleClass
        {
            public virtual void F() { }    
        }
        
        public class AGenericClass<TLeft, TRight>
        {
            public AGenericClass<TRight, TLeft> Swap()
            {
                return new AGenericClass<TRight, TLeft> {
                    Left = Right,
                    Right = Left
                };
            }
            
            public TLeft Left { get; set; }
            public TRight Right { get; set; }
        }
    }
}