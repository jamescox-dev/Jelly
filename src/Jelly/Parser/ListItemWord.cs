namespace Jelly.Parser;

public class ListItemParser
{
    static readonly StrValue ValueKeyword = new("value");

    static readonly IParser[] Parsers = new IParser[]
    {
        new SimpleWordParser(),
        new QuotedWordParser(false),
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