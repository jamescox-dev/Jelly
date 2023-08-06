namespace Jelly.Parser;

public class VariableParser : IParser
{
    readonly char? _terminatingChar;
    readonly bool _terminateAtOperator;
    readonly IParser _expressionParser;

    public VariableParser(IParser expressionParser, char? terminatingChar = null, bool terminateAtOperator = false)
    {
        _terminatingChar = terminatingChar;
        _terminateAtOperator = terminateAtOperator;
        _expressionParser = expressionParser;
    }

    public DictValue? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsVariableMarker))
        {
            int start = scanner.Position;
            scanner.AdvanceWhile(s => !(s.IsSpecialCharacter || IsTerminatingChar(s)));
            var nameEnd = scanner.Position;
            var name = scanner.Source[start..nameEnd];
            if (IsIndexer(scanner))
            {
                var indexers = new List<DictValue>();
                while (IsIndexer(scanner))
                {
                    var indexerStart = scanner.Position;
                    var isDictIndexer = scanner.AdvanceIf(s => s.IsDictIndexer);
                    var indexerExpression = _expressionParser.Parse(scanner)!;
                    var indexer = isDictIndexer
                        ? Node.DictIndexer(indexerStart, scanner.Position, indexerExpression)
                        : Node.ListIndexer(indexerStart, scanner.Position, indexerExpression);
                    indexers.Add(indexer);
                }
                return Node.Variable(start - 1, scanner.Position, name, indexers.ToArray());
            }
            else if (nameEnd > start)
            {
                return Node.Variable(name, start - 1, nameEnd);
            }
            else
            {
                throw new ParseError("A variable must have a name.")
                {
                    StartPosition = start - 1, EndPosition = start
                };
            }
        }
        return null;
    }

    static bool IsIndexer(Scanner scanner)
    {
        return scanner.IsExpressionBegin || scanner.IsDictIndexer;
    }

    bool IsTerminatingChar(Scanner scanner) =>
        scanner.IsDictIndexer
        || scanner.CurrentCharacter == _terminatingChar
        || (_terminateAtOperator && scanner.TryGetOperatorSymbol(out var _));
}