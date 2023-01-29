using Jelly.Values;
using Jelly.Errors;

namespace Jelly.Parser;

public class NestingWordParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        if (position < source.Length && config.IsNestingQuote(source[position]))
        {
            ++position;
            
            var depth = 1;
            var start = position;
            var escapeRun = 0;
            while (position < source.Length)
            {
                var ch = source[position];
                if (config.IsNestingQuote(ch) && (escapeRun % 2) == 0)
                {
                    ++depth;
                }
                else if (config.IsNestingEndQuote(ch) && (escapeRun % 2) == 0)
                {
                    --depth;
                    if (depth == 0)
                    {
                        ++position;
                        return Node.Literal(source[start .. (position - 1)].ToValue());
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
                ++position;
            }

            throw new ParseError("Unexpected end-of-input in nesting-word.");
        }
        return null;
    }
}