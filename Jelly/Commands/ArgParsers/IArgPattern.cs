namespace Jelly.Commands.ArgParsers;

public interface IArgPattern
{
    ArgPatternResult Parse(int position, ListValue args);
}