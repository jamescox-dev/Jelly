namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class WordParser : IParser
{
    readonly IParser[] _parsers;

    public WordParser(char? terminatingChar = null, ScriptParser? subscriptParser = null, ExpressionParser? expressionParser = null)
    {
        _parsers = new IParser[] 
        {
            new SimpleWordParser(terminatingChar),
            new QuotedWordParser(),
            new VariableParser(terminatingChar),
            subscriptParser ?? new ScriptParser(true),
            new CommentParser(),
            new NestingWordParser(),
            expressionParser ?? new ExpressionParser(subscriptParser),
        };
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        foreach (var parser in _parsers)
        {
            var node = parser.Parse(scanner);
            if (node is not null)
            {
                return node;
            }
        }
        return null;
    }
}