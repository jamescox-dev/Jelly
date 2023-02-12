using System.Globalization;
using Jelly.Errors;
using Jelly.Parser.Scanning;

namespace Jelly.Parser;


public class EscapeCharacterParser
{
    public string? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsEscapeCharacter))
        {
            if (!scanner.IsEof)
            {
                var ch = (char)scanner.SubstitedEscapeCharacter!;
                if (scanner.AdvanceIf(s => s.IsEscapeCharacter8bit))
                {
                    return ParseHexCode(scanner, 2);
                }
                else if (scanner.AdvanceIf(s => s.IsEscapeCharacter16bit))
                {
                    return ParseHexCode(scanner, 4);
                }
                else if (scanner.AdvanceIf(s => s.IsEscapeCharacter24bit))
                {
                    return ParseHexCode(scanner, 6);
                }
                scanner.Advance();
                return new string(ch, 1);
            }
            throw new ParseError("Unexpected end-of-input after escape-character.");
        }
        return null;
    }

    static string ParseHexCode(Scanner scanner, int length)
    {
        var hexCode = scanner.Substring(length);
        if (hexCode.Length == length)
        {
            if (int.TryParse(hexCode, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out var code))
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