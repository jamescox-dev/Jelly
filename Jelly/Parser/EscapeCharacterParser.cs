using System.Globalization;
using Jelly.Errors;

namespace Jelly.Parser;


public class EscapeCharacterParser
{
    public string? Parse(string source, ref int position, IParserConfig config)
    {
        if (position < source.Length && config.IsEscapeCharacter(source[position]))
        {
            ++position;
            if (position < source.Length)
            {
                var ch = source[position++];
                if (config.IsEscape8bit(ch))
                {
                    return ParseHexCode(source, ref position, 2);
                }
                else if (config.IsEscape16bit(ch))
                {
                    return ParseHexCode(source, ref position, 4);
                }
                else if (config.IsEscape24bit(ch))
                {
                    return ParseHexCode(source, ref position, 6);
                }
                else if (config.EscapeCharacterSubstitutions.ContainsKey(ch))
                {
                    ch = config.EscapeCharacterSubstitutions[ch];
                }
                return new string(ch, 1);
            }
            throw new ParseError("Unexpected end-of-input after escape-character.");
        }
        return null;
    }

    static string ParseHexCode(string source, ref int position, int length)
    {
        if (position + length - 1 < source.Length)
        {
            if (int.TryParse(source[position..(position + length)], NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out var code))
            {
                position += length;
                return char.ConvertFromUtf32(code);
            }
            else
            {
                throw new ParseError($"Invalid {length * 4}bit escape-character.");
            }
        }
        throw new ParseError($"Unexpected end-of-input after {length * 4}bit escape-character.");
    }
}