namespace Jelly.Shell.Extensions;

public record class Substring(string Source, int Start, int End)
{
    public override string ToString() => Source[Start..End];
}