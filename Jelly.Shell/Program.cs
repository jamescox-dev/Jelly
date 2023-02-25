namespace Jelly.Shell;

using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Shell.Io;

public class Program
{
    public static int Main(string[] args)
    {
        var historyFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "jelly", "history.json");
        var writer = new ConsoleWriter();
        var reader = new ReadLineReader();
        var historyManager = new ReadLineHistoryManager(historyFile);
        var shell = new Shell(
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

        Console.CancelKeyPress += (sender, args) =>
        {
            historyManager.SaveHistory();
        };

        if (args.Length == 0)
        {
            shell.Repl();
            return 0;
        }
        else
        {
            var filename = args[0];
            var script = File.ReadAllText(filename);
            return shell.RunScript(script);
        }
    }
}