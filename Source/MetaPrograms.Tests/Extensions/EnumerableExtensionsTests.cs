using System.Linq;
using MetaPrograms.Extensions;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [TestCase("A;B;C|A;B|A;B;C;D", "A;B")]
        [TestCase("A;B;C|B;C|A;B;C;D", "")]
        [TestCase("A|A;B|A;B;C", "A")]
        [TestCase("A;B;C|A;B|A", "A")]
        [TestCase("A;B;C|A;B|B", "")]
        [TestCase("A;B|A;B", "A;B")]
        [TestCase("A;B|", "")]
        [TestCase("|A;B", "")]
        [TestCase("||", "")]
        [TestCase("", "")]
        public void TestFindCommonPrefix(string inputString, string expectedOutputString)
        {
            var parsedInput = inputString.Split('|').Select(s => s.Split(';')).ToArray();

            var output = parsedInput.FindCommonPrefix();

            var actualStringOutput = string.Join(";", output);
            actualStringOutput.ShouldBe(expectedOutputString);
        }
    }
}
