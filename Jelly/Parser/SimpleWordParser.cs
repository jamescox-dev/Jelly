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
            else if (config.IsSpecialCharacter(ch) || config.GetOperatorAt(source, position) is not null)
            {
                break;
            }
            else
            {
                value.Append(ch);
                ++position;
            }
        }

        return start == position ? null : Node.Literal(value.ToString().ToValue());
    }
}