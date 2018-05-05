using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Shouldly;

namespace MetaPrograms.IntegrationTests
{
    public static class ShouldlyExtensions
    {
        public static void ShouldBeCodeEquivalentTo(this string actual, string expected)
        {
            var normalizedActual = NormalizeCode(actual);
            var normalizedExpected = NormalizeCode(expected);

            normalizedActual.ShouldBe(normalizedExpected);
        }

        public static string NormalizeCode(string code)
        {
            var tokens = code.Split(new[] {' ', '\t', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var normalized = string.Join(' ', tokens);
            return normalized;
        }

        public static void ShouldMatchTextFile(this Stream actual, string expectedFilePath)
        {
            
        }
    }
}
