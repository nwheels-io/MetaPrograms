using System;
using System.Collections;
using MetaPrograms.CSharp.Reader;
using MetaPrograms.CSharp.Tests.CompiledExamples;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.CSharp.Tests.Reader
{
    [TestFixture]
    public class SymbolExtensionsTests
    {
        public class SystemTypeMetadataNameTestCases : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] {TestTypeSymbols.TypeC0, "NS1.NS2.C0,Test", null};
                yield return new object[] {TestTypeSymbols.TypeC1, "NS1.NS2.C1,Test", null};
                yield return new object[] {TestTypeSymbols.TypeC2, "NS1.NS2.C1+C2,Test", null};
                yield return new object[] {TestTypeSymbols.TypeC3Open, "NS1.NS2.C3`2,Test", null};
                yield return new object[] {TestTypeSymbols.TypeC3OfC0C1, "NS1.NS2.C3`2[[NS1.NS2.C0,Test],[NS1.NS2.C1,Test]],Test", null};
                yield return new object[] {TestTypeSymbols.TypeC3OfIntString, "NS1.NS2.C3`2[[System.Int32],[System.String]],Test", null};
                yield return new object[] {TestTypeSymbols.TypeC6, "NS1.NS3.C6,Test", null};
                yield return new object[] {
                    TestTypeSymbols.ClosedGenericTypeFromCompiledExamples, 
                    "MetaPrograms.CSharp.Tests.CompiledExamples.GenericClassTwo`2[" + 
                        "[MetaPrograms.CSharp.Tests.CompiledExamples.IInterfaceOne,MetaPrograms.CSharp.Tests]" +
                        "," +
                        "[MetaPrograms.CSharp.Tests.CompiledExamples.IInterfaceTwo,MetaPrograms.CSharp.Tests]" +
                    "],MetaPrograms.CSharp.Tests",
                    typeof(GenericClassTwo<IInterfaceOne, IInterfaceTwo>)
                };
            }
        }

        private static readonly TypeSymbolsForTest TestTypeSymbols = new TypeSymbolsForTest();

        [TestCaseSource(typeof(SystemTypeMetadataNameTestCases))]
        public void CanGetSystemTypeMetadataName(INamedTypeSymbol symbol, string expectedMetadataName, Type expectedClrType)
        {
            var actualMetadataName = symbol.GetSystemTypeMetadataName();
            var actualClrType = (expectedClrType != null ? Type.GetType(actualMetadataName, throwOnError: false) : null);

            actualMetadataName.ShouldBe(expectedMetadataName);
            actualClrType.ShouldBeSameAs(expectedClrType);
        }
    }
}