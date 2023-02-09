namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;
using System.Text;

public class SimpleWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();

    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        var start = scanner.Position;
        var value = new StringBuilder();

        while (scanner.Position < scanner.Source.Length)
        {
            var ch = scanner.Source[scanner.Position];
            var escapedCh = EscapeCharacterParser.Parse(scanner, config);
            if (escapedCh is not null)
            {
                value.Append(escapedCh);
            }
            else if (config.IsSpecialCharacter(ch) || config.GetOperatorAt(scanner.Source, scanner.Position) is not null)
            {
                break;
            }
            else
            {
                value.Append(ch);
                scanner.Advance();
            }
        }

        return start == scanner.Position ? null : Node.Literal(value.ToString().ToValue());
    }
}