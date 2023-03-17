namespace Jelly.Parser;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class VariableParser : IParser
{
    readonly char? _terminatingChar;

    public VariableParser(char? terminatingChar = null)
    {
        _terminatingChar = terminatingChar;
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsVariableMarker))
        {
            int start;
            if (scanner.AdvanceIf(s => s.IsVariableBegin))
            {
                start = scanner.Position;
                scanner.AdvanceWhile(s => !s.IsVariableEnd);
                scanner.Advance();
                return Node.Variable(scanner.Source[start..(scanner.Position - 1)]);
            }
            else
            {
                start = scanner.Position;
                scanner.AdvanceWhile(s => !(s.IsSpecialCharacter || IsTerminatingChar(s)));
                if (scanner.Position > start)
                {
                    return Node.Variable(scanner.Source[start..scanner.Position]);
                }
                else
                {
                    throw new ParseError("A variable must have a name.");
                }
            }
        }
        return null;
    }

    public bool IsTerminatingChar(Scanner scanner) => scanner.CurrentCharacter == _terminatingChar;
}