using Jelly.Ast;
using Jelly.Parser.Scanning;
using Jelly.Values;

namespace Jelly.Parser;

public class OperatorParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        if (scanner.TryGetOperator(out var op))
        {
            scanner.Advance(op.Length);
            return Node.Literal(op.ToValue());
        }
        return null;
    }
}