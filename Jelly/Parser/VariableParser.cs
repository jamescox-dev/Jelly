namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

public class VariableParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        if (scanner.Position < scanner.Source.Length && config.IsVariableCharacter(scanner.Source[scanner.Position]))
        {
            scanner.Advance();
            int start;
            if (scanner.Position < scanner.Source.Length && config.IsVariableDelimiter(scanner.Source[scanner.Position]))
            {
                scanner.Advance();
                start = scanner.Position;
                while (scanner.Position < scanner.Source.Length && !config.IsVariableEndDelimiter(scanner.Source[scanner.Position]))
                {
                    scanner.Advance();
                }
                scanner.Advance();
                return Node.Variable(scanner.Source[start..(scanner.Position - 1)]);
            }
            else
            {
                start = scanner.Position;
                while (scanner.Position < scanner.Source.Length && !(config.IsSpecialCharacter(scanner.Source[scanner.Position]) || config.GetOperatorAt(scanner.Source, scanner.Position) is not null))
                {
                    scanner.Advance();
                }
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
}