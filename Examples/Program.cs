namespace Jelly.Shell;

using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Shell.Io;
using System.IO;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var total = 0;
        var passed = 0;
        foreach (var dir in Directory.EnumerateDirectories(".")) 
        {
            var scriptFileName = Path.Join(dir, Path.GetFileName(dir) + ".jly");
            var outFileName = Path.Join(dir, "out.txt");

            if (File.Exists(scriptFileName) && File.Exists(outFileName))
            {
                ++total;
                Console.WriteLine($"-- Running:  {scriptFileName} {new string('-', Math.Max(0, 64 - scriptFileName.Length))}--");
                
                var success = false;
                try
                {
                    var script = File.ReadAllText(scriptFileName);
                    var expectedOutput = File.ReadAllText(outFileName);
                    
                    var inFileName = Path.Join(dir, "in.txt");
                    var writer = new StringShellWriter();
                    IReader reader = File.Exists(inFileName) ? new FileShellReader(inFileName, writer) : new ConsoleReader();
                    
                    var shell = BuildShell(reader, writer);
                    shell.RunScript(script);

                    var output = writer.Output.ToString();
                    success = output == expectedOutput;
                }
                catch
                {
                }
                
                if (success)
                {
                    ++passed;
                }
                Console.WriteLine($"-------------------------------------------------------------------[ {(success ? "SUCCESS" : " FAIL! ")} ]--");
                Console.WriteLine();
            }
        }
        Console.WriteLine();
        Console.WriteLine($"Total test run:  {total},  failed:  {total - passed}, passed:  {passed}");
    }

    public static Shell BuildShell(IReader reader, IWriter writer)
    {
        var historyManager = new NoHistoryManager();
        return new Shell(
                reader,
                writer,
                new Scope(),
                new ScriptParser(),
                new Evaluator(),
                new ILibrary[] { 
                    new Jelly.Experimental.CoreLibrary(),
                    new Jelly.Library.CoreLibrary(),
                    new Jelly.Experimental.MathLibrary(),
                    new Jelly.Shell.Library.CoreIoLibrary(reader, writer)
                },
                new ShellConfig(),
                historyManager);
    }
}

class FileShellReader : IReader
{
    readonly TextReader _input;
    readonly IWriter _writer;

    public FileShellReader(string fileName, IWriter writer)
    {
        _input = new StreamReader(new FileStream(fileName, FileMode.Open, FileAccess.Read));
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
        Console.Write(message);
    }

    public void WriteLine(string message)
    {
        Output.AppendLine(message);
        Console.WriteLine(message);
    }
}