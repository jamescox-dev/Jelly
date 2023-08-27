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
                    var indexerExpression = isDictIndexer && !scanner.IsExpressionBegin
                        ? ParseDictIndexerKey(scanner)
                        : _expressionParser.Parse(scanner);
                    if (indexerExpression is not null)
                    {
                        var indexer = isDictIndexer
                            ? Node.DictIndexer(indexerStart, scanner.Position, indexerExpression)
                            : Node.ListIndexer(indexerStart, scanner.Position, indexerExpression);
                        indexers.Add(indexer);
                    }
                    else
                    {
                        throw new ParseError("dict indexer missing key expression.")
                        {
                            StartPosition = scanner.Position - 1,
                            EndPosition = scanner.Position
                        };
                    }
                }
                return Node.Variable(start - 1, scanner.Position, name, indexers.ToArray());
            }
            else if (nameEnd > start)
            {
                return Node.Variable(start - 1, nameEnd, name);
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

    DictValue? ParseDictIndexerKey(Scanner scanner)
    {
        var start = scanner.Position;
        if (scanner.AdvanceWhile(s => !(s.IsSpecialCharacter || IsTerminatingChar(s))) > 0)
        {
            return Node.Literal(start, scanner.Position, scanner.Source[start..scanner.Position]);
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