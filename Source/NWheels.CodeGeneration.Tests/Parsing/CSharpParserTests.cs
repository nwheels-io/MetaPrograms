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
    public class CSharpParserTests
    {
        [SetUp]
        public void Setup()
        {
        }
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void CanParseSimpleMethod()
        {
            //-- arrange

            var input = MakeSourceStream(@"
                namespace NS1
                {
                    class C1 
                    {
                        void ParseMe()
                        {
                            int num = 123;
                        }
                    }
                }
            ");

            var parser = new CSharpCodeParser();

            //-- act

            parser.Parse(input);

            //-- assert

            var classType = parser.Types.Single(t => t.FullName == "NS1.C1");
            var method = classType.Members.OfType<MethodMember>().Single(m => m.Name == "ParseMe");

            method.Visibility.ShouldBe(MemberVisibility.Private);
            method.Signature.IsVoid.ShouldBeTrue();
            method.Signature.Parameters.ShouldBeEmpty();

            method.Body.Locals.Count.ShouldBe(1);
            method.Body.Locals[0].Name.ShouldBe("num");
            method.Body.Locals[0].Type.ClrBinding.ShouldBe(typeof(int));

            method.Body.Statements.Count.ShouldBe(1);
            method.Body.Statements[0].ShouldBeOfType<ExpressionStatement>();

            var assignment = (AssignmentExpression)((ExpressionStatement)method.Body.Statements[0]).Expression;

            assignment.ShouldNotBeNull();
            assignment.Left.ShouldNotBeOfType<LocalVariableExpression>();
            ((LocalVariableExpression) assignment.Left).Variable.ShouldBeSameAs(method.Body.Locals[0]);
            ((LocalVariableExpression) assignment.Left).Variable.Type.ShouldBeSameAs(method.Body.Locals[0].Type);

            assignment.Right.ShouldNotBeOfType<ConstantExpression>();
            ((ConstantExpression) assignment.Right).Value.ShouldBe(123);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        private MemoryStream MakeSourceStream(string code)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(code);
            writer.Flush();

            stream.Position = 0;
            return stream;
        }
    }
}
