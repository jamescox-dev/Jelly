using Jelly.Values;
using Jelly.Errors;
using Jelly.Ast;
using Jelly.Parser.Scanning;

namespace Jelly.Parser;

public class NestingWordParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsNestingQuoteBegin))
        {   
            var depth = 1;
            var start = scanner.Position;
            var escapeRun = 0;
            while (scanner.Position < scanner.Source.Length)
            {
                var ch = scanner.Source[scanner.Position];
                if (scanner.IsNestingQuoteBegin && (escapeRun % 2) == 0)
                {
                    ++depth;
                }
                else if (scanner.IsNestingQuoteEnd && (escapeRun % 2) == 0)
                {
                    --depth;
                    if (depth == 0)
                    {
                        scanner.Advance();
                        return Node.Literal(scanner.Source[start .. (scanner.Position - 1)].ToValue());
                    }
                }
                if (scanner.IsEscapeCharacter)
                {
                    ++escapeRun;
                }
                else
                {
                    escapeRun = 0;
                }
                scanner.Advance();
            }

            throw Error.MissingEndToken("Unexpected end-of-input in nesting-word.");
        }
        return null;
    }
}