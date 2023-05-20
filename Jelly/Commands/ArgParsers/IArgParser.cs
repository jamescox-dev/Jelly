namespace Jelly.Commands.ArgParsers;


public interface IArgParser
{
    DictionaryValue Parse(ListValue args, int index=0);
}