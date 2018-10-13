using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiffMatchPatch;
using NUnit.Framework;
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
            var actualReader = new StreamReader(actual);
            var actualText = actualReader.ReadToEnd();
            var expectedText = File.ReadAllText(expectedFilePath);

            diff_match_patch algorithm = new diff_match_patch();

            var diffs = algorithm.diff_main(expectedText, actualText);
            algorithm.diff_cleanupSemantic(diffs);

            if (diffs.Any(IsSignificantDiff))
            {
                var message = BuildDiffMessage(expectedFilePath, diffs);
                Assert.Fail(message);
            }
        }

        private static bool IsSignificantDiff(Diff diff)
        {
            return (diff.operation != Operation.EQUAL && diff.text.Trim().Length > 0);
        }

        private static string BuildDiffMessage(string filePath, List<Diff> diffs)
        {
            var message = new StringBuilder();
            message.AppendLine($"------ file doesn't match: {Path.GetFileName(filePath)} ------");

            var allLines = BuildDiffLines(diffs);
            var lineNo = 1;

            foreach (var diff in diffs)
            {
                if (IsSignificantDiff(diff))
                {
                    message.AppendLine($"--- at line {lineNo}, {diff.operation}: {diff.text}");
                    AppendDiffLines(allLines, lineNo - 1, lineNo + 1, message);
                }

                var lineSkipCount = diff.text.Count(c => c == '\n');
                lineNo += lineSkipCount;
            }

            message.AppendLine($"------ end match ------");
            return message.ToString();
        }

        private static void AppendDiffLines(List<string> lines, int fromLine, int toLine, StringBuilder output)
        {
            output.AppendLine();

            for (int lineNo = fromLine; lineNo <= toLine; lineNo++)
            {
                if (lineNo < 1 || lineNo > lines.Count)
                {
                    continue;
                }

                output.AppendLine($"    Line {lineNo,4} : {lines[lineNo - 1]}");
            }

            output.AppendLine();
        }

        private static List<string> BuildDiffLines(List<Diff> diffs)
        {
            var stream = new MemoryStream();

            WriteDiffLines(diffs, stream);

            stream.Position = 0;

            return ReadDiffLines(stream);
        }

        private static void WriteDiffLines(List<Diff> diffs, MemoryStream stream)
        {
            var writer = new StreamWriter(stream);

            foreach (var diff in diffs)
            {
                var significant = IsSignificantDiff(diff);

                if (significant)
                {
                    writer.Write($"[{diff.operation}]");
                }

                writer.Write(diff.text);

                if (significant)
                {
                    writer.Write($"[/{diff.operation}]");
                }
            }

            writer.Flush();
        }

        private static List<string> ReadDiffLines(MemoryStream stream)
        {
            var allLines = new List<string>();
            var reader = new StreamReader(stream);

            string line;

            while ((line = reader.ReadLine()) != null)
            {
                allLines.Add(line);
            }

            return allLines;
        }
    }
}
