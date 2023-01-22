using Jelly.Values;

namespace Jelly.Parser;

public class WordParser : IParser
{
    static readonly IParser[] Parsers = new IParser[] 
    {
        new SimpleWordParser(),
        new VariableParser(),
    };
    
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        foreach (var parser in Parsers)
        {
            var node = parser.Parse(source, ref position, config);
            if (node is not null)
            {
                return node;
            }
        }
        return null; // TODO:  Think this case should throw!
    }
}