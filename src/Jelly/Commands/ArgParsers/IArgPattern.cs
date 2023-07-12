namespace Jelly.Commands.ArgParsers;

public interface IArgPattern
{
    ArgPatternResult Match(int position, ListValue args);
}