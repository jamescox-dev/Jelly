namespace Jelly.Parser;

public class WordParser : IParser
{
    readonly IEnumerable<IParser> _parsers;

    public WordParser(char? terminatingChar = null, ScriptParser? subscriptParser = null, ExpressionParser? expressionParser = null, bool terminateAtOperator = false)
    {
        subscriptParser ??= new ScriptParser(true);
        expressionParser ??= new ExpressionParser(subscriptParser);
        var parsers = new List<IParser>
        {
            new SimpleWordParser(terminatingChar, terminateAtOperator),
            new QuotedWordParser(),
            new VariableParser(expressionParser, terminatingChar, terminateAtOperator),
            subscriptParser,
            new CommentParser(),
            new NestingWordParser(),
            expressionParser,
        };
        if (terminateAtOperator)
        {
            parsers.Add(new OperatorParser());
        }
        _parsers = parsers;
    }

    public DictValue? Parse(Scanner scanner)
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