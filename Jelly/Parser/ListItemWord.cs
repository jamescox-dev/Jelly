namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class ListItemParser
{
    static readonly StringValue ValueKeyword = new StringValue("value");

    static readonly IParser[] Parsers = new IParser[] 
    {
        new SimpleWordParser(),
        new QuotedWordParser(false),
        new OperatorParser(),
        new NestingWordParser(),
    };
    
    public Value? Parse(Scanner scanner)
    {
        foreach (var parser in Parsers)
        {
            var node = parser.Parse(scanner);
            if (node is not null)
            {
                return node[ValueKeyword];
            }
        }
        return null;
    }
}