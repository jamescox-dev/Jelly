namespace Jelly.Parser;

using System.Text;
using Jelly.Errors;
using Jelly.Values;

public class QuotedWordParser : IParser
{
    static readonly EscapeCharacterParser EscapeCharacterParser = new();
    static readonly VariableParser VariableParser = new();
    static readonly ScriptParser ScriptParser = new(true);

    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var parts = new List<DictionaryValue>();
        var literal = new StringBuilder();

        if (position < source.Length && config.IsQuote(source[position]))
        {
            var openingQuote = source[position++];
            while (position < source.Length)
            {
                var ch = source[position];
                var escapedCh = EscapeCharacterParser.Parse(source, ref position, config);
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
                    var script = ScriptParser.Parse(source, ref position, config);
                    if (script is not null)
                    {
                        parts.Add(script);
                        continue;
                    }
                    throw new NotImplementedException(); // This should not happen.
                }
                else if (ch == openingQuote)
                {
                    ++position;
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
                    ++position;
                }
            }
            throw new ParseError($"Unexpected end-of-input in quoted-word.");
        }

        return null;
    }
}