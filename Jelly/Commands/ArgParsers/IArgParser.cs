namespace Jelly.Commands.ArgParsers;

public interface IArgParser
{
    DictionaryValue Parse(string commandName, ListValue args);
}