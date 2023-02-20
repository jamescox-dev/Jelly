namespace Jelly.Shell;

using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Shell.Io;

public class Program
{
    public static void Main(string[] args)
    {
        var historyFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "jelly", "history.json");
        var writer = new ConsoleWriter();
        var reader = new ReadLineReader();
        var historyManager = new ReadLineHistoryManager(historyFile);

        Console.CancelKeyPress += (sender, args) =>
        {
            historyManager.SaveHistory();
        };

        new Shell(
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
            historyManager).Repl();
    }
}