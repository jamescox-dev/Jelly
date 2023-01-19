namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;
using System.Text;

public class SimpleWordParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var start = position;
        var value = new StringBuilder();

        while (position < source.Length)
        {
            var ch = source[position];
            if (config.IsWordSeparator(ch))
            {
                break;
            }
            else if (config.IsEscapeCharacter(ch))
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
            else
            {
                value.Append(ch);
                ++position;
            }
        }

        return start == position ? null : NodeBuilder.Shared.Literal(value.ToString().ToValue());
    }
}