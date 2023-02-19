namespace Jelly.Shell;

using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Shell.Io;

public class Program
{
    public static void Main(string[] args)
    {
        var writer = new ConsoleWriter();
        var reader = new ReadLineReader();
        
        new Shell(
            reader,
            writer,
            new Scope(),
            new ScriptParser(),
            new Evaluator(),
            new ILibrary[] { 
                new Jelly.Experimental.CoreLibrary(),
                new Jelly.Library.CoreLibrary(),
                new Jelly.Shell.Library.CoreIoLibrary(reader, writer)
            },
            new ShellConfig()).Repl();
    }
}