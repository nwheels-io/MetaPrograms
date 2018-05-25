using NUnit.Framework;
using Shouldly;

namespace CommonExtensions.Tests
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
    }
}
