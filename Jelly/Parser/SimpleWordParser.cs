namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;
using System.Text;

public class SimpleWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();

    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var start = position;
        var value = new StringBuilder();

        while (position < source.Length)
        {
            var ch = source[position];
            var escapedCh = EscapeCharacterParser.Parse(source, ref position, config);
            if (escapedCh is not null)
            {
                value.Append(escapedCh);
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