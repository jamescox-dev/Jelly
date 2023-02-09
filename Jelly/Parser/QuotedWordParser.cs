namespace Jelly.Parser;

using System.Text;
using Jelly.Errors;
using Jelly.Values;

public class QuotedWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly VariableParser VariableParser = new();
    static readonly ScriptParser ScriptParser = new(true);

    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        var parts = new List<DictionaryValue>();
        var literal = new StringBuilder();

        if (scanner.Position < scanner.Source.Length && config.IsQuote(scanner.Source[scanner.Position]))
        {
            var openingQuote = scanner.Source[scanner.Position];
            scanner.Advance();
            while (scanner.Position < scanner.Source.Length)
            {
                var ch = scanner.Source[scanner.Position];
                var escapedCh = EscapeCharacterParser.Parse(scanner, config);
                if (escapedCh is not null)
                {
                    literal.Append(escapedCh);
                }
                else if (config.IsScriptCharacter(ch))
                {
                    if (literal.Length != 0)
                    {
                        parts.Add(Node.Literal(literal.ToString().ToValue()));
                        literal.Clear();
                    }
                    var script = ScriptParser.Parse(scanner, config);
                    if (script is not null)
                    {
                        parts.Add(script);
                        continue;
                    }
                    throw new NotImplementedException(); // This should not happen.
                }
                else if (ch == openingQuote)
                {
                    scanner.Advance();
                    if (literal.Length != 0)
                    {
                        parts.Add(Node.Literal(literal.ToString().ToValue()));
                        literal.Clear();
                    }
                    return Node.Composite(parts.ToArray());
                }
                else
                {
                    literal.Append(ch);
                    scanner.Advance();
                }
            }
            throw new ParseError($"Unexpected end-of-input in quoted-word.");
        }

        return null;
    }
}