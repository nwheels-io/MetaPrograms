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

            var visitor = new TypeDiscoverySymbolVisitor(compilation, modelBuilder, discoveredTypeReaders);
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

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:C2"
            });
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

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:C2",
                "ClassReader:C3"
            });
        }

        [Test]
        public void NestedClasses()
        {
            var typeReaders = VisitCode(@"
                class C1 { 
                    class C2 {     
                        class C3 { }
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:C2",
                "ClassReader:C3"
            });
        }

        [Test]
        public void IncludeBaseClass()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : ClassOne { }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:ClassOne"
            });
        }

        [Test]
        public void IncludeImplementedInterfaces()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : IInterfaceOne, IInterfaceTwo { }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            });
        }

        [Test]
        public void IncludeGenericArguments()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 : GenericClassTwo<IInterfaceOne, IInterfaceTwo> {  }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:GenericClassTwo",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            });
        }

        [Test]
        public void IncludeTypesOfFields()
        {
            var typeReaders = VisitCode(@"
                #pragma warning disable CS0169 // unused fields
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    IInterfaceOne f1; 
                    IInterfaceTwo f2; 
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            });
        }

        [Test]
        public void IncludeTypesOfProperties()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    IInterfaceOne P1 { get; set; } 
                    IInterfaceTwo P2 { get; set; } 
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "InterfaceReader:IInterfaceOne",
                "InterfaceReader:IInterfaceTwo",
            });
        }

        [Test]
        public void IncludeTypesOfEventDelegates()
        {
            var typeReaders = VisitCode(@"
                using System;
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    public void F() { if (E1 != null) { } }
                    public event Action<ClassOne> E1;
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:ClassOne",
                "ClassReader:Action",
            });
        }

        [Test]
        public void IncludeTypesInMethodSignatures()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    ClassOne F(GenericClassTwo<ClassThree, ClassFour> p) {
                        return null;
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:ClassOne",
                "ClassReader:GenericClassTwo",
                "ClassReader:ClassThree",
                "ClassReader:ClassFour",
            });
        }

        [Test]
        public void IncludeTypesOfLocalVariables()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    void F() {
                        var c = new ClassOne();
                        while (true) {
                            var z = new ClassThree();
                        }
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "ClassReader:ClassOne",
                "ClassReader:ClassThree",
            });
        }

        [Test]
        public void IncludeSystemObjectType()
        {
            var typeReaders = VisitCode(@"
                class C1 { 
                    void F(object value) { }
                }
            ");

            SelectReaderSymbolPairsFrom(typeReaders).ShouldBe(new[] {
                "ClassReader:C1",
                "ClassReader:Object",
                "StructReader:Int32",
                "StructReader:Boolean",
                "ClassReader:String",
                "ClassReader:Type",
            }, ignoreOrder: true);
        }

        [Test]
        public void IncludeStandardLanguageTypes()
        {
            var typeReaders = VisitCode(@"
                class C1 { 
                    int F(string s) {
                        return 123;                    
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:C1",
                "StructReader:Int32",
                "ClassReader:String"
            });
        }

        [Test]
        public void IncludeTypesFromLanguageConstructs()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    void F(object value) {
                        if (value is ClassOne one) {
                            using (var disposable = new DisposableFive()) {
                                for (int i = 0; i < 10 ; i++) { }
                            }
                        }
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:Object",
                "ClassReader:ClassOne",
                "ClassReader:DisposableFive",
                "StructReader:Int32",
            });
        }

        [Test]
        public void IncludeTypeFromNewExpression()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    void F(object value) {
                        using (new DisposableFive()) {
                        }
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:DisposableFive",
            });
        }

        [Test]
        public void IncludeTypesFromObjectCreationExpressions()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    void F() {
                        using (new DisposableFive()) {
                        }
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:DisposableFive"
            });
        }

        [Test]
        public void IncludeTypesFromArrayElements()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    ClassOne[] F() {
                        return null;
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:ClassOne",
            });
        }

        [Test]
        public void IncludeTypesFromArrayCreationExpression()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    object F() {
                        return new ClassOne[0];
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:ClassOne",
            });
        }

        [Test]
        public void IncludeTypesFromArrayInitializer()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    object[] F() {
                        return new object[] { new ClassThree(), new ClassFour() };
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:ClassThree",
                "ClassReader:ClassFour"
            });
        }

        [Test]
        public void IncludeTypesFromObjectInitializer()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    ClassOne F() {
                        return new ClassOne() {     
                            First = new ClassThree(),
                            Second = new ClassFour()
                        };
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:ClassThree",
                "ClassReader:ClassFour"
            });
        }

        [Test]
        public void IncludeTypesFromStaticMethodInvocations()
        {
            var typeReaders = VisitCode(@"
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;
                class C1 { 
                    void F() {
                        StaticSix.M1();
                        StaticSix.M2<ClassThree, ClassFour>();
                    }
                }
            ");

            AssertReaderSymbolPairs(typeReaders, shouldContain: new[] {
                "ClassReader:StaticSix",
                "ClassReader:ClassThree",
                "ClassReader:ClassFour"
            });
        }

        private IEnumerable<IPhasedTypeReader> VisitCode(string csharpCode)
        {
            var compilation = CompileCode(csharpCode);
            var results = new List<IPhasedTypeReader>();
            var visitor = new TypeDiscoverySymbolVisitor(compilation, new CodeModelBuilder(), results);

            compilation.GlobalNamespace.Accept(visitor);

            return results;
        }

        private void AssertReaderSymbolPairs(IEnumerable<IPhasedTypeReader> results, string[] shouldContain)
        {
            var pairs = SelectReaderSymbolPairsFrom(results);

            foreach (var value in shouldContain)
            {
                pairs.ShouldContain(value);
            }
        }

        private string[] SelectReaderSymbolPairsFrom(IEnumerable<IPhasedTypeReader> results)
        {
            return results
                .Select(reader => $"{reader.GetType().Name}:{reader.TypeSymbol.Name}")
                .ToArray();
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
