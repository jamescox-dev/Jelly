namespace Jelly.Parser;

using System.Text;

public class QuotedWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly VariableParser VariableParser = new();
    static readonly ScriptParser ScriptParser = new(true);

    readonly bool _allowSubstitutions;

    public QuotedWordParser(bool allowSubstitutions=true)
    {
        _allowSubstitutions = allowSubstitutions;
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        var parts = new List<DictionaryValue>();
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
                        : Node.Reposition(parts[0], start, scanner.Position);
                }
                else
                {
                    literal.Append(scanner.CurrentCharacter);
                    scanner.Advance();
                }
            }
            throw Error.MissingEndToken($"Unexpected end-of-input in quoted-word.");
        }

        return null;

        void FlushCurrentLiteral(int literalEnd)
        {
            if (literal.Length != 0)
            {
                parts.Add(Node.Literal(literal.ToString(), literalStart, literalEnd));
                literalStart = literalEnd;
                literal.Clear();
            }
        }
    }
}