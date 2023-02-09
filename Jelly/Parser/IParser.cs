namespace Jelly.Parser;

using Jelly.Values;

public interface IParser
{
    DictionaryValue? Parse(Scanner scanner, IParserConfig config);
}