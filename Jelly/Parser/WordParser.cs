namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class WordParser : IParser
{
    static readonly IParser[] Parsers = new IParser[] 
    {
        new SimpleWordParser(),
        new QuotedWordParser(),
        new VariableParser(),
        new ScriptParser(true),
        new CommentParser(),
        new OperatorParser(),
        new NestingWordParser(),
    };
    
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        foreach (var parser in Parsers)
        {
            var node = parser.Parse(scanner, config);
            if (node is not null)
            {
                return node;
            }
        }
        return null;
    }
}