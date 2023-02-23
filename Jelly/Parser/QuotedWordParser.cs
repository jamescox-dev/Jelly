namespace Jelly.Parser;

using System.Text;
using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class QuotedWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly VariableParser VariableParser = new();
    static readonly ScriptParser ScriptParser = new(true);

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
                else if (scanner.IsScriptBegin)
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
                    return Node.Composite(parts.ToArray());
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