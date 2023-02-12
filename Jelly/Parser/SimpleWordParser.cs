namespace Jelly.Parser;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;
using System.Text;

public class SimpleWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();

    public DictionaryValue? Parse(Scanner scanner)
    {
        var start = scanner.Position;
        var value = new StringBuilder();

        while (!scanner.IsEof)
        {
            var escapedCh = EscapeCharacterParser.Parse(scanner);
            if (escapedCh is not null)
            {
                value.Append(escapedCh);
            }
            else if (scanner.IsSpecialCharacter)
            {
                break;
            }
            else
            {
                value.Append(scanner.CurrentCharacter);
                scanner.Advance();
            }
        }

        return start == scanner.Position ? null : Node.Literal(value.ToString().ToValue());
    }
}