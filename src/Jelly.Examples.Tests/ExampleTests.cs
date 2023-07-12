namespace Jelly.Examples.Tests;

using System.Collections.Generic;
using System.IO;
using System.Text;
using Jelly.Runtime;
using Jelly.Shell;

[TestFixture]
public class ExampleTests
{
    static readonly string ExamplesDir = Path.Join("..", "..", "..", "..", "..", "examples");

    static readonly string TestCaseDataDir = Path.Join("..", "..", "..", "TestCases");

    [TestCaseSource(nameof(ExampleTestCases))]
    public void TestExample(string script, string input, string expectedOutput)
    {
        var writer = new StringShellWriter();
        var reader = new StringShellReader(input, writer);
        var historyManager = new NoHistoryManager();
        var shell = new Shell(
                reader,
                writer,
                new Env(),
                new ILibrary[] {
                    new Jelly.Experimental.Library.CollectionsLibrary(),
                    new Jelly.Library.CollectionsLibrary(),
                    new Jelly.Experimental.Library.CoreLibrary(),
                    new Jelly.Library.CoreLibrary(),
                    new Jelly.Experimental.Library.MathLibrary(),
                    new Jelly.Library.MathLibrary(),
                    new Jelly.Experimental.Library.StringLibrary(),
                    new Jelly.Library.StringLibrary(),
                    new Jelly.Experimental.Library.StringLibrary(),
                    new Jelly.Experimental.Library.UtilsLibrary(),
                    new Jelly.Library.UtilsLibrary(),
                    new Jelly.Library.IoLibrary(new Jelly.Experimental.Library.UnsafeIoLibraryProvider()),
                    new Jelly.Shell.Library.CoreIoLibrary(reader, writer)
                },
                new ShellConfig(),
                historyManager);

        shell.RunScript(script);

        writer.Output.ToString().Should().Be(expectedOutput);
    }

    static IEnumerable<object[]> ExampleTestCases()
    {
        foreach (var scriptFileName in Directory.GetFiles(ExamplesDir, "*.jly", SearchOption.AllDirectories))
        {
            var testDataDir = Path.Join(TestCaseDataDir, Path.GetRelativePath(ExamplesDir, scriptFileName));
            if (Directory.Exists(testDataDir))
            {
                var script = File.ReadAllText(scriptFileName);

                if (TryGetTestCaseData(testDataDir, out var input, out var output))
                {
                    yield return new[] { script, input, output };
                }

                foreach (var testCaseDirs in Directory.GetDirectories(testDataDir))
                {
                    if (TryGetTestCaseData(testCaseDirs, out var testCaseInput, out var testCaseOutput))
                    {
                        yield return new[] { script, testCaseInput, testCaseOutput };
                    }
                }
            }
        }
    }

    static bool TryGetTestCaseData(string path, out string input, out string output)
    {
        input = string.Empty;
        output = null!;

        var outFileName = Path.Join(path, "out.txt");
        if (File.Exists(outFileName))
        {
            output = File.ReadAllText(outFileName).ReplaceLineEndings();

            var inFileName = Path.Join(path, "in.txt");
            if (File.Exists(inFileName))
            {
                input = File.ReadAllText(inFileName).ReplaceLineEndings();
            }

            return true;
        }

        return false;
    }
}

class StringShellReader : IReader
{
    readonly TextReader _input;
    readonly IWriter _writer;

    public StringShellReader(string input, IWriter writer)
    {
        _input = new StringReader(input);
        _writer = writer;
    }

    public string ReadLine()
    {
        var line = _input.ReadLine() ?? string.Empty;
        _writer.WriteLine(line);
        return line;
    }
}

class StringShellWriter : IWriter
{
    public StringBuilder Output { get; } = new();

    public void Write(string message)
    {
        Output.Append(message);
    }

    public void WriteLine(string message)
    {
        Output.AppendLine(message);
    }
}