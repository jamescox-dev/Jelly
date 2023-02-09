using System.Globalization;
using Jelly.Errors;

namespace Jelly.Parser;


public class EscapeCharacterParser
{
    public string? Parse(Scanner scanner, IParserConfig config)
    {
        if (scanner.Position < scanner.Source.Length && config.IsEscapeCharacter(scanner.Source[scanner.Position]))
        {
            scanner.Advance();
            if (scanner.Position < scanner.Source.Length)
            {
                var ch = scanner.Source[scanner.Position];
                scanner.Advance();
                if (config.IsEscape8bit(ch))
                {
                    return ParseHexCode(scanner, 2);
                }
                else if (config.IsEscape16bit(ch))
                {
                    return ParseHexCode(scanner, 4);
                }
                else if (config.IsEscape24bit(ch))
                {
                    return ParseHexCode(scanner, 6);
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

    static string ParseHexCode(Scanner scanner, int length)
    {
        if (scanner.Position + length - 1 < scanner.Source.Length)
        {
            if (int.TryParse(scanner.Source[scanner.Position..(scanner.Position + length)], NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out var code))
            {
                scanner.Advance(length);
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