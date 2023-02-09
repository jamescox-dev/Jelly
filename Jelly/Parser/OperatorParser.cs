using Jelly.Values;

namespace Jelly.Parser;

public class OperatorParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        var op = config.GetOperatorAt(scanner.Source, scanner.Position);
        if (op is not null)
        {
            scanner.Advance(op.Length);
            return Node.Literal(op.ToValue());
        }
        return null;
    }
}