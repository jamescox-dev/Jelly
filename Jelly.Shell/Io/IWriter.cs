namespace Jelly.Shell.Io;

public interface IWriter
{
    void Write(string output);
    
    void WriteLine(string output);
}