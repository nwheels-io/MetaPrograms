using System;
using System.Collections.Generic;
using System.Linq;
using MetaPrograms.Adapters.Roslyn.Reader;
using MetaPrograms.CodeModel.Imperative;
using MetaPrograms.CodeModel.Imperative.Members;
using Microsoft.CodeAnalysis;
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

            reader.ReadName();

            // assert

            var type = reader.TypeMember;

            type.Name.ShouldBe("MyClass");
            type.Namespace.ShouldBe("MyApp.MyTest");
            type.FullName.ShouldBe("MyApp.MyTest.MyClass");
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

            reader.ReadAncestors();

            // assert

            var type = reader.TypeMember;
            type.BaseType.Get().ShouldNotBeNull();
            GetSymbolName(type.BaseType).ShouldBe("MyBase");
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

            reader.ReadAncestors();

            // assert

            var type = reader.TypeMember;
            type.Interfaces.Count.ShouldBe(2);
            type.Interfaces.Select(GetSymbolName).ShouldBe(new[] { "IService1", "IService2" }, ignoreOrder: true);
        }

        [Test]
        public void CanReadMemberDeclarations()
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

            reader.ReadMemberDeclarations();

            // assert

            var type = reader.TypeMember;
            type.Members.Select(m => m.Get()).OfType<ConstructorMember>().Select(GetSymbolName).ShouldBe(new[] { ".ctor" });
            type.Members.Select(m => m.Get()).OfType<FieldMember>().Select(GetSymbolName).ShouldBe(new[] { "f1" });
            type.Members.Select(m => m.Get()).OfType<MethodMember>().Select(GetSymbolName).ShouldBe(new[] { "M1" });
            type.Members.Select(m => m.Get()).OfType<PropertyMember>().Select(GetSymbolName).ShouldBe(new[] { "P1" });
            type.Members.Select(m => m.Get()).OfType<EventMember>().Select(GetSymbolName).ShouldBe(new[] { "E1" });
        }

        [Test]
        public void CanReadTypeAttributes()
        {
            // arrange

            var code = @"
                using System;
                using MetaPrograms.Adapters.Roslyn.Tests.CompiledExamples;

                [Serializable, Seventh(123, DayOfWeek.Tuesday)]
                class MyClass { }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(
                code, 
                "MyClass",
                setupAllReaders: allReaders => allReaders.ForEach(r => r.ReadName()));

            // act

            reader.ReadAttributes();

            // assert

            var type = reader.TypeMember;
            type.Attributes
                .Select(a => a.AttributeType.Get().Name)
                .ShouldBe(new[] { "SerializableAttribute", "SeventhAttribute" }, ignoreOrder: true);
        }

        private string GetSymbolName(AbstractMember member)
        {
            return member.Bindings.OfType<ISymbol>().Single().Name;
        }

        private string GetSymbolName<T>(MemberRef<T> member)
            where T : AbstractMember
            => GetSymbolName(member.Get());

        private (ClassReader, CodeModelBuilder) GetClassReaderFromCode(
            string csharpCode, 
            string className, 
            Action<List<IPhasedTypeReader>> setupAllReaders = null)
        {
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(
                csharpCode,
                references: new[] {
                    this.GetType().Assembly.Location
                });

            var typeSymbol = compilation.GetTypeByMetadataName(className);
            typeSymbol.ShouldNotBeNull($"Type symbol '{className}' could not be found in compilation.");

            var modelBuilder = new CodeModelBuilder();
            var allReaders = new List<IPhasedTypeReader>();
            var discoveryVisitor = new TypeDiscoverySymbolVisitor(compilation, modelBuilder, allReaders);

            compilation.GlobalNamespace.Accept(discoveryVisitor);
            setupAllReaders?.Invoke(allReaders);
            
            var reader = allReaders.OfType<ClassReader>().FirstOrDefault(r => r.TypeSymbol.Equals(typeSymbol));
            reader.ShouldNotBeNull($"ClassReader for '{className}' was not registered.");

            allReaders.ForEach(r => r.RegisterProxy());
            return (reader, modelBuilder);
        }
    }
}