namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

public class WordParser : IParser
{
    static readonly IParser[] Parsers = new IParser[] 
    {
        new SimpleWordParser(),
        new VariableParser(),
        new ScriptParser(true),
        new CommentParser(),
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
        return null;
    }
}