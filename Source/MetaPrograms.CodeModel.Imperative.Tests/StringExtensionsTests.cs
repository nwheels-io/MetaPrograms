using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace MetaPrograms.CodeModel.Imperative.Tests
{
    public class StringExtensionsTests
    {
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