using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MetaPrograms.IntegrationTests
{
    [TestFixture]
    public class ShouldlyExtensionsTests
    {
        [TestCase(
            "A{B{C('hello')}}",
            ExpectedResult = "A{B{C('hello')}}")]
        [TestCase(
            "A\r\n{\r\n\tB\r\n\t{\r\n\t\tC('hello')\r\n\t}\r\n}\r\n", 
            ExpectedResult = "A { B { C('hello') } }")]
        public string TestNormalizeCode(string input)
        {
            return ShouldlyExtensions.NormalizeCode(input);
        }
    }
}
