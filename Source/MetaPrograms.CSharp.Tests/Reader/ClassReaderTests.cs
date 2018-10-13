using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MetaPrograms.CSharp.Reader;
using MetaPrograms;
using MetaPrograms.Members;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.CSharp.Tests.Reader
{
    [TestFixture]
    public class ClassReaderTests
    {
        [TearDown]
        public void AfterEach()
        {
            CodeContextBase.Cleanup();
        }

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

            type.Name.ToString().ShouldBe("MyClass");
            type.Namespace.ShouldBe("MyApp.MyTest");
            type.FullName.ShouldBe("MyApp.MyTest.MyClass");
        }
        
        [Test]
        public void CanReadGenerics()
        {
            // arrange
            
            var code = @"
                namespace MyApp {
                    interface IService1 { }
                    interface IService2 { }
                    class C0<T1, T2> { }
                    class C1 : C0<IService1, IService2> { }
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(
                code, 
                "MyApp.C1",
                walkSymbols: c1 => c1.BaseType,
                setupAllReaders: allReaders => allReaders.ForEach(r => r.ReadName()));

            // act

            reader.ReadGenerics();

            // assert

            var c1Base = reader.TypeMember;
            c1Base.IsGenericType.ShouldBeTrue();
            c1Base.IsGenericDefinition.ShouldBeFalse();
            c1Base.GenericParameters.Select(arg => arg.Name.ToString()).ShouldBe(new[] { "T1", "T2" });
            c1Base.GenericArguments.Select(arg => arg.Name.ToString()).ShouldBe(new[] { "IService1", "IService2" });
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
            type.BaseType.ShouldNotBeNull();
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
            type.Members.OfType<ConstructorMember>().Select(GetSymbolName).ShouldBe(new[] { ".ctor" });
            type.Members.OfType<FieldMember>().Select(GetSymbolName).ShouldBe(new[] { "f1" });
            type.Members.OfType<MethodMember>().Select(GetSymbolName).ShouldBe(new[] { "M1" });
            type.Members.OfType<PropertyMember>().Select(GetSymbolName).ShouldBe(new[] { "P1" });
            type.Members.OfType<EventMember>().Select(GetSymbolName).ShouldBe(new[] { "E1" });
        }

        [Test]
        public void CanReadGenericsInMemberDeclarations()
        {
            // arrange
            
            var code = @"
                using System;
                namespace MyApp {
                    class C1 { }
                    class C2 {
                         Action<C1> Callback { get; set; }
                    }
                }
            ";

            (ClassReader reader, CodeModelBuilder model) = GetClassReaderFromCode(
                code, 
                "MyApp.C2",
                setupAllReaders: allReaders => {
                    allReaders.ForEach(r => r.ReadName());
                    allReaders.ForEach(r => r.RegisterProxy());
                    allReaders.ForEach(r => r.ReadGenerics());
                });

            // act

            reader.ReadMemberDeclarations();

            // assert

            var type = reader.TypeMember;
            var property = type.Members
                .OfType<PropertyMember>()
                .Single(p => p.Name.ToString() == "Callback");

            var propertyType = property.PropertyType;
            
            propertyType.IsGenericType.ShouldBe(true);
            propertyType.IsGenericDefinition.ShouldBe(false);
            propertyType.GenericParameters.Count.ShouldBe(1);
            propertyType.GenericArguments.Count.ShouldBe(1);
            propertyType.GenericArguments[0].Name.ToString().ShouldBe("C1");
        }
        
        [Test]
        public void CanReadTypeAttributes()
        {
            // arrange

            var code = @"
                using System;
                using MetaPrograms.CSharp.Tests.CompiledExamples;

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
                .Select(a => a.AttributeType.Name.ToString())
                .ShouldBe(new[] { "SerializableAttribute", "SeventhAttribute" }, ignoreOrder: true);
        }

        private string GetSymbolName(AbstractMember member)
        {
            return member.Bindings.OfType<ISymbol>().Single().Name;
        }

        private (ClassReader, CodeModelBuilder) GetClassReaderFromCode(
            string csharpCode, 
            string className, 
            Func<INamedTypeSymbol, INamedTypeSymbol> walkSymbols = null, 
            Action<List<IPhasedTypeReader>> setupAllReaders = null)
        {
            var compilation = TestWorkspaceFactory.CompileCodeOrThrow(
                csharpCode,
                references: new[] {
                    this.GetType().Assembly.Location
                });

            var originSymbol = compilation.GetTypeByMetadataName(className);
            var typeSymbol = walkSymbols?.Invoke(originSymbol) ?? originSymbol; 
            typeSymbol.ShouldNotBeNull($"Type symbol '{className}' could not be found in compilation.");

            var modelBuilder = new CodeModelBuilder(compilation);
            var allReaders = new List<IPhasedTypeReader>();
            var discoveryVisitor = new TypeDiscoverySymbolVisitor(modelBuilder, allReaders);

            var readerContext = new CodeReaderContext(modelBuilder.GetCodeModel(), null, LanguageInfo.Entries.CSharp());

            compilation.GlobalNamespace.Accept(discoveryVisitor);
            setupAllReaders?.Invoke(allReaders);

            var reader = allReaders.OfType<ClassReader>().FirstOrDefault(r => r.TypeSymbol.Equals(typeSymbol));
            reader.ShouldNotBeNull($"ClassReader for '{className}' was not registered.");

            allReaders.ForEach(r => r.RegisterProxy());
            return (reader, modelBuilder);
        }
    }
}