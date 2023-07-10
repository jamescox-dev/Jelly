namespace Jelly.Shell;

using System;
using Jelly.Runtime;

public static class Program
{
    public static int Main(string[] args)
    {
        var historyFile = Path.Join(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), ".config", "jelly", "history.json");
        var writer = new ConsoleWriter();
        IReader reader = Console.IsInputRedirected ? new ConsoleReader() : new ReadLineReader();
        var historyManager = new ReadLineHistoryManager(historyFile);
        var shell = new Shell(
                reader,
                writer,
                new Runtime.Environment(),
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

        Console.CancelKeyPress += (sender, args) => historyManager.SaveHistory();

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