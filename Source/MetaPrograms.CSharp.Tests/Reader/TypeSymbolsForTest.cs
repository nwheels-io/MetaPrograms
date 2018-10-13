using Microsoft.CodeAnalysis;

namespace MetaPrograms.CSharp.Tests.Reader
{
    public class TypeSymbolsForTest
    {
        public const string SourceCode = @"
                using MetaPrograms.CSharp.Tests.CompiledExamples;
                namespace NS1.NS2 { 
                    public class C0 { }
                    public class C1 {
                        public class C2 { }         
                    }
                    public class C3<T1, T2> { }
                    public class C4 : C3<C0, C1> { }
                    public class C5 : C3<int, string> { }
                }
                namespace NS1.NS3 {
                    public class C6 : GenericClassTwo<IInterfaceOne, IInterfaceTwo> { }
                } 
            ";
            
        public TypeSymbolsForTest()
        {
            this.Compilation = TestWorkspaceFactory.CompileCodeOrThrow(SourceCode, references: ThisAssemblyLocation);
        }
            
        public Compilation Compilation { get; }
        public INamedTypeSymbol TypeC0 => Compilation.GetTypeByMetadataName("NS1.NS2.C0");
        public INamedTypeSymbol TypeC1 => Compilation.GetTypeByMetadataName("NS1.NS2.C1");
        public INamedTypeSymbol TypeC2 => Compilation.GetTypeByMetadataName("NS1.NS2.C1+C2");
        public INamedTypeSymbol TypeC4 => Compilation.GetTypeByMetadataName("NS1.NS2.C4");
        public INamedTypeSymbol TypeC5 => Compilation.GetTypeByMetadataName("NS1.NS2.C5");
        public INamedTypeSymbol TypeC3OfC0C1 => TypeC4.BaseType;
        public INamedTypeSymbol TypeC3OfIntString => TypeC5.BaseType;
        public INamedTypeSymbol TypeC3Open => TypeC3OfC0C1.OriginalDefinition;
        public INamedTypeSymbol TypeC6 => Compilation.GetTypeByMetadataName("NS1.NS3.C6");
        public INamedTypeSymbol GenericClassTwo => Compilation.GetTypeByMetadataName("NS1.NS3.C6");
        public INamedTypeSymbol ClosedGenericTypeFromCompiledExamples => TypeC6.BaseType;
        
        public static string[] ThisAssemblyLocation => new[] { typeof(TypeSymbolsForTest).Assembly.Location };
    }
}