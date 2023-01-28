namespace Jelly.Parser;

using System.Text;
using Jelly.Errors;
using Jelly.Values;

// TODO:  Add variable, script and expression substitutions.

public class QuotedWordParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var value = new StringBuilder();

        if (position < source.Length && config.IsQuote(source[position]))
        {
            ++position;
            while (position < source.Length)
            {
                var ch = source[position];
                if (config.IsEscapeCharacter(ch))
                {
                    if (position + 1 < source.Length)
                    {
                        value.Append(source[++position]);
                        ++position;
                    }
                    else
                    {
                        throw new ParseError($"Unexpected end-of-input after escape-character '{ch}'.");
                    }
                }
                else if (config.IsQuote(ch))
                {
                    ++position;
                    return Node.Literal(value.ToString().ToValue());
                }
                else
                {
                    value.Append(ch);
                    ++position;
                }
            }
            throw new ParseError($"Unexpected end-of-input in quoted-word.");
        }

        return null;
    }
}