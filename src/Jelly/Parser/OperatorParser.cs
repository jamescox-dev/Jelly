namespace Jelly.Parser;

public class OperatorParser : IParser
{
    public DictValue? Parse(Scanner scanner)
    {
        if (scanner.TryGetOperatorSymbol(out var op))
        {
            var start = scanner.Position;
            scanner.Advance(op.Length);
            return Node.Literal(start, scanner.Position, op.ToValue());
        }
        return null;
    }
}