namespace Jelly.Parser;

public class OperatorParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner)
    {
        if (scanner.TryGetOperatorSymbol(out var op))
        {
            var start = scanner.Position;
            scanner.Advance(op.Length);
            return Node.Literal(op.ToValue(), start, scanner.Position);
        }
        return null;
    }
}