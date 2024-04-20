namespace Jelly.Parser;

public class QuotedWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly ScriptParser ScriptParser = new(true);
    readonly bool _allowSubstitutions;

    public QuotedWordParser(bool allowSubstitutions=true)
    {
        _allowSubstitutions = allowSubstitutions;
    }

    public DictValue? Parse(Scanner scanner)
    {
        var parts = new List<DictValue>();
        var literal = new StringBuilder();
        var literalStart = 0;
        if (scanner.IsQuote)
        {
            var start = scanner.Position;
            var openingQuote = (char)scanner.CurrentCharacter!;
            scanner.Advance();
            literalStart = scanner.Position;
            while (!scanner.IsEof)
            {
                var escapedCh = EscapeCharacterParser.Parse(scanner);
                if (escapedCh is not null)
                {
                    literal.Append(escapedCh);
                }
                else if (scanner.IsScriptBegin && _allowSubstitutions)
                {
                    FlushCurrentLiteral(scanner.Position);
                    var script = ScriptParser.Parse(scanner);
                    if (script is not null)
                    {
                        parts.Add(script);
                        literalStart = scanner.Position;
                        continue;
                    }
                    throw new NotImplementedException(); // This should not happen.
                }
                else if (scanner.AdvanceIf(s => s.CurrentCharacter == openingQuote))
                {
                    FlushCurrentLiteral(scanner.Position - 1);
                    return _allowSubstitutions
                        ? Node.Composite(start, scanner.Position, parts.ToArray())
                        : Node.Reposition(parts.Any() ? parts[0] : Node.Literal(string.Empty), start, scanner.Position);
                }
                else
                {
                    literal.Append(scanner.CurrentCharacter);
                    scanner.Advance();
                }
            }
            throw new MissingEndTokenError($"Unexpected end-of-input in quoted-word.")
            {
                StartPosition = scanner.Position,
                EndPosition = scanner.Position
            };
        }

        return null;

        void FlushCurrentLiteral(int literalEnd)
        {
            if (literal.Length != 0)
            {
                parts.Add(Node.Literal(literalStart, literalEnd, literal.ToString()));
                literalStart = literalEnd;
                literal.Clear();
            }
        }
    }
}
