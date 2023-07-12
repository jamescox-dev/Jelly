namespace Jelly.Shell.Io;

public class ConsoleReader : IReader
{
    public string ReadLine()
    {
        var line = Console.ReadLine() ?? string.Empty;
        Console.WriteLine();
        return line;
    }
}