using MetaPrograms.Extensions;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestCase("yzab", "yz", "ab")]
        [TestCase("yzab", "xy", "yzab")]
        [TestCase("byza", "yz", "byza")]
        [TestCase("abyz", "yz", "abyz")]
        [TestCase("yz", "yz", "yz")]
        [TestCase("", "yz", "")]
        [TestCase(null, "yz", null)]
        public void CanTrimPrefix(string input, string prefix, string expectedOutput)
        {
            input.TrimPrefix(prefix).ShouldBe(expectedOutput);
        }

        [TestCase("abcd", "cd", "ab")]
        [TestCase("abcd", "xy", "abcd")]
        [TestCase("bcda", "cd", "bcda")]
        [TestCase("cdab", "cd", "cdab")]
        [TestCase("cd", "cd", "cd")]
        [TestCase("", "cd", "")]
        [TestCase(null, "cd", null)]
        public void CanTrimSuffix(string input, string suffix, string expectedOutput)
        {
            input.TrimSuffix(suffix).ShouldBe(expectedOutput);
        }

        [TestCase("abZZcd", "ZZ", "ab")]
        [TestCase("abcd", "ZZ", "abcd")]
        [TestCase("abZZ", "ZZ", "ab")]
        [TestCase("ZZab", "ZZ", "")]
        [TestCase("", "ZZ", "")]
        [TestCase(null, "ZZ", null)]
        public void CanTrimEndStartingWith(string input, string subString, string expectedOutput)
        {
            input.TrimEndStartingWith(subString).ShouldBe(expectedOutput);
        }
    }
}
