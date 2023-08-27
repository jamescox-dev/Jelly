namespace Jelly.Parser;

using System.Globalization;



public class EscapeCharacterParser
{
    public string? Parse(Scanner scanner)
    {
        var start = scanner.Position;
        if (scanner.AdvanceIf(s => s.IsEscapeCharacter))
        {
            if (!scanner.IsEof)
            {
                var ch = (char)scanner.SubstitutedEscapeCharacter!;
                if (scanner.AdvanceIf(s => s.IsEscapeCharacter8bit))
                {
                    return ParseHexCode(scanner, 2, start);
                }
                else if (scanner.AdvanceIf(s => s.IsEscapeCharacter16bit))
                {
                    return ParseHexCode(scanner, 4, start);
                }
                else if (scanner.AdvanceIf(s => s.IsEscapeCharacter24bit))
                {
                    return ParseHexCode(scanner, 6, start);
                }
                scanner.Advance();
                return new string(ch, 1);
            }
            throw new ParseError("Unexpected end-of-input after escape-character.")
            {
                StartPosition = start,
                EndPosition = scanner.Position
            };
        }
        return null;
    }

    static string ParseHexCode(Scanner scanner, int length, int start)
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
                throw new ParseError($"Invalid {length * 4}bit escape-character.")
                {
                    StartPosition = start,
                    EndPosition = start + 2 + length
                };
            }
        }
        throw new ParseError($"Unexpected end-of-input after {length * 4}bit escape-character.")
        {
            StartPosition = start,
            EndPosition = start + 2 + hexCode.Length
        };
    }
}