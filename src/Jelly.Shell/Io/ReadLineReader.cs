namespace Jelly.Shell.Io;

public class ReadLineReader : IReader
{
    public string ReadLine() => System.ReadLine.Read();
}