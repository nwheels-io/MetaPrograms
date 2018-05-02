using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using NWheels.CodeGeneration.CodeModel.Expressions;
using NWheels.CodeGeneration.CodeModel.Members;
using NWheels.CodeGeneration.CodeModel.Statements;
using NWheels.CodeGeneration.Parsing;

namespace NWheels.CodeGeneration.Tests.Parsing
{
    public class CSharpCompilationParserTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanParseSingleTree()
        {
            //-- arrange

            var input = ParserTestHelpers.MakeSourceStream(@"
                namespace NS1 {
                    class C1 {
                    }
                }
            ");

            var parser = new CSharpCompilationParser();

            //-- act

            parser.Parse(input);
            parser.Analyze();

            //-- assert

            var classType = parser.Types.Single(t => t.FullName == "NS1.C1");
            classType.TypeKind.ShouldBe(TypeMemberKind.Class);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanParseMultipleTrees()
        {
            //-- arrange

            var input1 = ParserTestHelpers.MakeSourceStream(@"
                namespace NS1 {
                    class C1 {
                    }
                }
            ");
            var input2 = ParserTestHelpers.MakeSourceStream(@"
                using NS1;
                namespace NS2 {
                    class C2 : C1 {
                    }
                }
            ");

            var parser = new CSharpCompilationParser();

            //-- act

            parser.Parse(input1);
            parser.Parse(input2);
            parser.Analyze();

            //-- assert

            var classC1 = parser.Types.Single(t => t.FullName == "NS1.C1");
            classC1.TypeKind.ShouldBe(TypeMemberKind.Class);

            var classC2 = parser.Types.Single(t => t.FullName == "NS2.C2");
            classC2.TypeKind.ShouldBe(TypeMemberKind.Class);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanParseWithReferencesToProgrammingModel()
        {
            //-- arrange

            var input = ParserTestHelpers.MakeSourceStream(@"
                using NWheels.CodeGeneration.Tests.Parsing;
                namespace NS1 {
                    [ExampleProgrammingModel.Example]
                    class C1 : ExampleProgrammingModel.ExampleBase {
                    }
                }
            ");

            var parser = new CSharpCompilationParser();

            //-- act

            parser.AddReference(this.GetType().Assembly.Location);
            parser.Parse(input);
            parser.Analyze();

            //-- assert

            var classC1 = parser.Types.Single(t => t.FullName == "NS1.C1");
            classC1.TypeKind.ShouldBe(TypeMemberKind.Class);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanReportSourceCodeErrors()
        {
            //-- arrange

            var input = ParserTestHelpers.MakeSourceStream(@"
                namespace NS1 {
                    [ExampleProgrammingModel.Example]
                    class C1 : ExampleProgrammingModel.ExampleBase {
                    }
                }
            ");

            var parser = new CSharpCompilationParser();

            //-- act

            parser.Parse(input);
            var exception = Should.Throw<SourceCodeErrorsException>(() => {
                parser.Analyze();
            });

            //-- assert

            exception.Diagnostics.ShouldContain(diag => diag.Descriptor.Id == "CS0246", expectedCount: 2);
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        //var method = classType.Members.OfType<MethodMember>().Single(m => m.Name == "ParseMe");

        //method.Visibility.ShouldBe(MemberVisibility.Private);
        //method.Signature.IsVoid.ShouldBeTrue();
        //method.Signature.Parameters.ShouldBeEmpty();

        //method.Body.Locals.Count.ShouldBe(1);
        //method.Body.Locals[0].Name.ShouldBe("num");
        //method.Body.Locals[0].Type.ClrBinding.ShouldBe(typeof(int));

        //method.Body.Statements.Count.ShouldBe(1);
        //method.Body.Statements[0].ShouldBeOfType<ExpressionStatement>();

        //var assignment = (AssignmentExpression)((ExpressionStatement)method.Body.Statements[0]).Expression;

        //assignment.ShouldNotBeNull();
        //assignment.Left.ShouldNotBeOfType<LocalVariableExpression>();
        //((LocalVariableExpression)assignment.Left).Variable.ShouldBeSameAs(method.Body.Locals[0]);
        //((LocalVariableExpression)assignment.Left).Variable.Type.ShouldBeSameAs(method.Body.Locals[0].Type);

        //assignment.Right.ShouldNotBeOfType<ConstantExpression>();
        //((ConstantExpression)assignment.Right).Value.ShouldBe(123);
    }
}
