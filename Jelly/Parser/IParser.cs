namespace Jelly.Parser;

using Jelly.Parser.Scanning;
using Jelly.Values;

public interface IParser
{
    DictionaryValue? Parse(Scanner scanner);
}