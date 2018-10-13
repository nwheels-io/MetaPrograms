#if false

using NWheels.Compilation.Adapters.Roslyn.SyntaxEmitters;
using MetaPrograms.Expressions;
using MetaPrograms.Members;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NWheels.Compilation.Adapters.Roslyn.UnitTests.SyntaxEmitters
{
    [TestFixture]
    public class AttributeSyntaxEmitterTests
    {
        [Test]
        public void AttributeWithNoValues()
        {
            //-- arrange 

            var attribute = new AttributeDescription() {
                AttributeType = typeof(TestAttributes.AttributeOne)
            };

            //-- act

            var syntax = AttributeSyntaxEmitter.EmitSyntax(attribute);

            //-- assert

            syntax.Should().BeEquivalentToCode("TestAttributes.AttributeOne");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AttributeWithConstructorArguments()
        {
            //-- arrange

            var attribute = new AttributeDescription() {
                AttributeType = typeof(TestAttributes.AttributeOne)
            };

            attribute.ConstructorArguments.Add(new ConstantExpression() { Value = 123 });
            attribute.ConstructorArguments.Add(new ConstantExpression() { Value = "ABC" });

            //-- act

            var syntax = AttributeSyntaxEmitter.EmitSyntax(attribute);

            //-- assert

            syntax.Should().BeEquivalentToCode("TestAttributes.AttributeOne(123, \"ABC\")");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AttributeWithPropertyValues()
        {
            //-- arrange

            var attribute = new AttributeDescription() {
                AttributeType = typeof(TestAttributes.AttributeOne)
            };

            attribute.PropertyValues.Add(new PropertyValue() { Name = "First", Value = new ConstantExpression() { Value = 123 } });
            attribute.PropertyValues.Add(new PropertyValue() { Name = "Second", Value = new ConstantExpression() { Value = "ABC" } });

            //-- act

            var syntax = AttributeSyntaxEmitter.EmitSyntax(attribute);

            //-- assert

            syntax.Should().BeEquivalentToCode("TestAttributes.AttributeOne(First = 123, Second = \"ABC\")");
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        [Test]
        public void AttributeWithMixOfArgumentsAndProperties()
        {
            //-- arrange

            var attribute = new AttributeDescription() {
                AttributeType = typeof(TestAttributes.AttributeOne)
            };

            attribute.ConstructorArguments.Add(new ConstantExpression() { Value = 123 });
            attribute.ConstructorArguments.Add(new ConstantExpression() { Value = "ABC" });
            attribute.PropertyValues.Add(new PropertyValue() { Name = "First", Value = new ConstantExpression() { Value = 456 } });
            attribute.PropertyValues.Add(new PropertyValue() { Name = "Second", Value = new ConstantExpression() { Value = "DEF" } });

            //-- act

            var syntax = AttributeSyntaxEmitter.EmitSyntax(attribute);

            //-- assert

            syntax.Should().BeEquivalentToCode("TestAttributes.AttributeOne(123, \"ABC\", First = 456, Second = \"DEF\")");
        }
    }
}

namespace TestAttributes
{
    public class AttributeOne : Attribute
    {
    } 
}

#endif