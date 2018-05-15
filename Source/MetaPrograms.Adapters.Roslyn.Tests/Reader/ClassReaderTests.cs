using System;
using System.Linq;
using MetaPrograms.Adapters.Roslyn.Reader;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Adapters.Roslyn.Tests.Reader
{
    [TestFixture]
    public class ClassReaderTests
    {
        [Test]
        public void CanReadNameAndNamespace()
        {
            // arrange
            
            var code = @"
                namespace MyApp.MyTest {
                    class MyClass { }
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(code, "MyApp.MyTest.MyClass");

            // act

            var typeMember = reader.Read();
            
            // assert
            
            typeMember.Name.ShouldBe("MyClass");
            typeMember.Namespace.ShouldBe("MyApp.MyTest");
            typeMember.FullName.ShouldBe("MyApp.MyTest.MyClass");
        }
        
        [Test]
        public void CanReadBaseType()
        {
            // arrange
            
            var code = @"
                namespace MyApp {
                    class MyBase { }
                    class MyConcrete : MyBase { }
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(code, "MyApp.MyConcrete");

            // act

            var concrete = reader.Read();
            
            // assert

            concrete.BaseType.ShouldNotBeNull();
            concrete.BaseType.FullName.ShouldBe("MyApp.MyBase");
        }
        
        [Test]
        public void CanReadImplementedInterfaces()
        {
            // arrange
            
            var code = @"
                namespace MyApp {
                    interface IService1 { }
                    interface IService2 { }
                    class MyConcrete : IService1, IService2 { }
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(code, "MyApp.MyConcrete");

            // act

            var concrete = reader.Read();
            
            // assert

            concrete.Interfaces.Count.ShouldBe(2);
            concrete.Interfaces.Select(t => t.FullName).ShouldBe(new[] { "MyApp.IService1", "MyApp.IService2" }, ignoreOrder: true);
        }

        [Test]
        public void CanReadMethodDeclarations()
        {
            // arrange
            
            var code = @"
                using System;
                class MyClass {
                    private int f1 = 123; 
                    public MyClass() { }
                    public void M1() { E1?.Invoke(); }
                    internal int P1 => f1;
                    public event Action E1;
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(code, "MyClass");

            // act

            var concrete = reader.Read();
            
            // assert
            
            concrete.Members.OfType<ConstructorMember>().Select(m => m.Name).ShouldBe(new[] { "MyClass" });
            concrete.Members.OfType<FieldMember>().Select(m => m.Name).ShouldBe(new[] { "f1" });
            concrete.Members.OfType<MethodMember>().Select(m => m.Name).ShouldBe(new[] { "M1" });
            concrete.Members.OfType<PropertyMember>().Select(m => m.Name).ShouldBe(new[] { "P1" });
            concrete.Members.OfType<EventMember>().Select(m => m.Name).ShouldBe(new[] { "E1" });
        }
        
        private (ClassReader, CodeModelBuilder) GetClassReaderFromCode(string csharpCode, string className)
        {
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(
                csharpCode,
                references: new[] {
                    this.GetType().Assembly.Location
                });

            var typeSymbol = compilation.GetTypeByMetadataName(className);
            typeSymbol.ShouldNotBeNull($"Type symbol '{className}' could not be found in compilation.");

            var syntaxTree = typeSymbol.DeclaringSyntaxReferences.First().SyntaxTree;
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            
            var modelBuilder = new CodeModelBuilder();
            var reader = new ClassReader(new TypeReaderMechanism(modelBuilder, semanticModel, typeSymbol));

            return (reader, modelBuilder);
        }
    }
}