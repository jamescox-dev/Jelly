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

        if (scanner.IsQuote)
        {
            var openingQuote = (char)scanner.CurrentCharacter!;
            scanner.Advance();
            while (!scanner.IsEof)
            {
                var escapedCh = EscapeCharacterParser.Parse(scanner);
                if (escapedCh is not null)
                {
                    literal.Append(escapedCh);
                }
                else if (scanner.IsScriptBegin && _allowSubstitutions)
                {
                    FlushCurrentLitral();
                    var script = ScriptParser.Parse(scanner);
                    if (script is not null)
                    {
                        parts.Add(script);
                        continue;
                    }
                    throw new NotImplementedException(); // This should not happen.
                }
                else if (scanner.AdvanceIf(s => s.CurrentCharacter == openingQuote))
                {
                    FlushCurrentLitral();
                    return _allowSubstitutions
                        ? Node.Composite(parts.ToArray())
                        : parts[0];
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

        void FlushCurrentLitral()
        {
            if (literal.Length != 0)
            {
                parts.Add(Node.Literal(literal.ToString().ToValue()));
                literal.Clear();
            }
        }
    }
}