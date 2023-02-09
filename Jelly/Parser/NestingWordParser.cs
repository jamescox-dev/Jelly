using Jelly.Values;
using Jelly.Errors;

namespace Jelly.Parser;

public class NestingWordParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        if (scanner.Position < scanner.Source.Length && config.IsNestingQuote(scanner.Source[scanner.Position]))
        {
            scanner.Advance();
            
            var depth = 1;
            var start = scanner.Position;
            var escapeRun = 0;
            while (scanner.Position < scanner.Source.Length)
            {
                var ch = scanner.Source[scanner.Position];
                if (config.IsNestingQuote(ch) && (escapeRun % 2) == 0)
                {
                    ++depth;
                }
                else if (config.IsNestingEndQuote(ch) && (escapeRun % 2) == 0)
                {
                    --depth;
                    if (depth == 0)
                    {
                        scanner.Advance();
                        return Node.Literal(scanner.Source[start .. (scanner.Position - 1)].ToValue());
                    }
                }
                if (config.IsEscapeCharacter(ch))
                {
                    ++escapeRun;
                }
                else
                {
                    escapeRun = 0;
                }
                scanner.Advance();
            }

            throw new ParseError("Unexpected end-of-input in nesting-word.");
        }
        return null;
    }
}