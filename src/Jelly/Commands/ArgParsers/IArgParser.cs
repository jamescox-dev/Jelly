namespace Jelly.Commands.ArgParsers;

public interface IArgParser
{
    DictValue Parse(string commandName, ListValue args);
}