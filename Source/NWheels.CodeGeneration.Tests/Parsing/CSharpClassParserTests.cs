using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NWheels.CodeGeneration.CodeModel.Members;
using NWheels.CodeGeneration.Parsing;
using Shouldly;

namespace NWheels.CodeGeneration.Tests.Parsing
{
    [TestFixture]
    public class CSharpClassParserTests
    {

        [Test]
        public void CanParseClassDeclaration()
        {
            var input = ParserTestHelpers.MakeSourceStream(@"
                namespace NS1 {
                    public abstract class C1 {
                    }
                    internal static class C2 {
                    }
                }
            ");

            var parser = new CSharpCompilationParser();

            //-- act

            parser.Parse(input);
            parser.Analyze();

            //-- assert

            var classC1 = parser.Types.Single(t => t.FullName == "NS1.C1");

            classC1.Namespace.ShouldBe("NS1");
            classC1.Name.ShouldBe("C1");
            classC1.TypeKind.ShouldBe(TypeMemberKind.Class);
            classC1.Visibility.ShouldBe(MemberVisibility.Public);
            classC1.Modifier.ShouldBe(MemberModifier.Abstract);

            var classC2 = parser.Types.Single(t => t.FullName == "NS1.C2");

            classC2.Namespace.ShouldBe("NS1");
            classC2.Name.ShouldBe("C2");
            classC2.TypeKind.ShouldBe(TypeMemberKind.Class);
            classC2.Visibility.ShouldBe(MemberVisibility.Internal);
            classC2.Modifier.ShouldBe(MemberModifier.Static);
        }
    }
}
