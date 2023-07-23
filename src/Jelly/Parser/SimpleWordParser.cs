namespace Jelly.Parser;

using System.Text;

public class SimpleWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly StrValue AssignmentOperator = new("=");

    readonly char? _terminatingChar;
    readonly bool _terminateAtOperator;

    public SimpleWordParser(char? terminatingChar = null, bool terminateAtOperator = false)
    {
        _terminatingChar = terminatingChar;
        _terminateAtOperator = terminateAtOperator;
    }

    public DictValue? Parse(Scanner scanner)
    {
        var start = scanner.Position;
        var value = new StringBuilder();

        if (scanner.AdvanceIf(s => s.IsAssignmentOperator))
        {
            return Node.Literal(AssignmentOperator, start, scanner.Position);
        }

        while (!scanner.IsEof)
        {
            var escapedCh = EscapeCharacterParser.Parse(scanner);
            if (escapedCh is not null)
            {
                value.Append(escapedCh);
            }
            else if (scanner.IsSpecialCharacter || IsTerminatingChar(scanner))
            {
                break;
            }
            else
            {
                value.Append(scanner.CurrentCharacter);
                scanner.Advance();
            }
        }

        return start == scanner.Position ? null : Node.Literal(value.ToString().ToValue(), start, scanner.Position);
    }

    public bool IsTerminatingChar(Scanner scanner) =>
        scanner.CurrentCharacter == _terminatingChar
        || (_terminateAtOperator && scanner.TryGetOperatorSymbol(out var _));
}