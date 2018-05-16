using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetaPrograms.Adapters.Roslyn.Reader;
using MetaPrograms.CodeModel.Imperative;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Adapters.Roslyn.Tests.Reader
{
    [TestFixture]
    public class TypeDiscoverySymbolVisitorTests
    {
        [Test]
        public void HowToUse()
        {
            // arrange

            var compilation = CompileCode(@"
                namespace NS1 {
                    class C1 { }
                }
            ");

            var modelBuilder = new CodeModelBuilder();
            var discoveredTypeReaders = new List<IPhasedTypeReader>();

            // act

            var visitor = new TypeDiscoverySymbolVisitor(modelBuilder, discoveredTypeReaders);
            compilation.GlobalNamespace.Accept(visitor);

            // assert

            var discoveredTypeNames = discoveredTypeReaders.Select(r => r.TypeSymbol.Name);
        }

        [Test]
        public void ClassesInGlobalNamespace()
        {
            var typeReaders = VisitCode(@"
                class C1 { }
                class C2 { }
            ");

            SelectReaderSymbolPairsFrom(typeReaders).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:C2"
            }, ignoreOrder: true);
        }

        [Test]
        public void ClassesInNamespaces()
        {
            var typeReaders = VisitCode(@"
                namespace NS1 {
                    class C1 { }
                }
                namespace NS1.NS2 {               
                    class C2 { }    
                    namespace NS3 {
                        class C3 { }
                    }
                }
            ");

            SelectReaderSymbolPairsFrom(typeReaders).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:C2",
                "ClassReader:C3"
            }, ignoreOrder: true);
        }

        [Test]
        public void NestedClasses()
        {
            var results = VisitCode(@"
                class C1 { 
                    class C2 {     
                        class C3 { }
                    }
                }
            ");

            SelectReaderSymbolPairsFrom(results).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:C2",
                "ClassReader:C3"
            }, ignoreOrder: true);
        }

        [Test]
        public void IncludeBaseClass()
        {
            var results = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : ClassOne { }
            ");

            SelectReaderSymbolPairsFrom(results).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:ClassOne"
            }, ignoreOrder: true);
        }

        [Test]
        public void IncludeImplementedInterfaces()
        {
            var results = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : IInterfaceOne, IInterfaceTwo { }
            ");

            SelectReaderSymbolPairsFrom(results).ShouldBe(new[] {
                "ClassReader:C1",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            }, ignoreOrder: true);
        }

        [Test]
        public void IncludeGenericArguments()
        {
            var results = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : GenericClassTwo<IInterfaceOne, IInterfaceTwo> {  }
            ");

            SelectReaderSymbolPairsFrom(results).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:GenericClassTwo",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            }, ignoreOrder: true);
        }

        private IEnumerable<IPhasedTypeReader> VisitCode(string csharpCode)
        {
            var compilation = CompileCode(csharpCode);
            var results = new List<IPhasedTypeReader>();
            var visitor = new TypeDiscoverySymbolVisitor(new CodeModelBuilder(), results);

            compilation.GlobalNamespace.Accept(visitor);

            return results;
        }

        private IEnumerable<string> SelectReaderSymbolPairsFrom(IEnumerable<IPhasedTypeReader> results)
        {
            return results.Select(reader => $"{reader.GetType().Name}:{reader.TypeSymbol.Name}");
        }

        private Compilation CompileCode(string csharpCode)
        {
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(
                csharpCode,
                references: new[] {
                    this.GetType().Assembly.Location
                });

            return compilation;
        }
    }
}
