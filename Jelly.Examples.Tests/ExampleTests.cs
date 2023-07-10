namespace Jelly.Examples.Tests;

using System.Collections.Generic;
using System.IO;
using System.Text;
using Jelly.Runtime;
using Jelly.Shell;

[TestFixture]
public class ExampleTests
{
    static readonly string ExamplesDir = Path.Join("..", "..", "..", "..", "Jelly.Examples");

    [TestCaseSource(nameof(Examples))]
    public void TestExample(string script, string input, string expectedOutput)
    {
        var writer = new StringShellWriter();
        var reader = new StringShellReader(input, writer);
        var historyManager = new NoHistoryManager();
        var shell = new Shell(
                reader,
                writer,
                new Environment(),
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

    static IEnumerable<object[]> Examples()
    {
        foreach (var scriptFileName in Directory.GetFiles(ExamplesDir))
        {
            var inFileName = Path.Join(ExamplesDir, "testdata", Path.GetFileName(scriptFileName), "in.txt");
            var outFileName = Path.Join(ExamplesDir, "testdata", Path.GetFileName(scriptFileName), "out.txt");
            if (scriptFileName.EndsWith(".jly") && File.Exists(outFileName))
            {
                var script = File.ReadAllText(scriptFileName);
                var input = string.Empty;
                var expectedOutput = File.ReadAllText(outFileName);
                if (File.Exists(inFileName))
                {
                    input = File.ReadAllText(inFileName);
                }
                yield return new[] { script, input, expectedOutput };
            }
        }
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
        //Console.Write(message);
    }

    public void WriteLine(string message)
    {
        Output.AppendLine(message);
        //Console.WriteLine(message);
    }
}