namespace Jelly.Parser;

using Jelly.Values;

public interface IParser
{
    DictionaryValue? Parse(string source, ref int position, IParserConfig config);
}