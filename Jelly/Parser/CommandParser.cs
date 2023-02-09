namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

// TODO:  Special form for commands that is just one expression node ().

public class CommandParser : IParser
{
    readonly WordParser _wordParser = new();

    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        var words = new List<DictionaryValue>();

        while (scanner.Position < scanner.Source.Length && !config.IsCommandSeparator(scanner.Source[scanner.Position]))
        {
            while (scanner.Position < scanner.Source.Length && config.IsWordSeparator(scanner.Source[scanner.Position]))
            {
                scanner.Advance();
            }

            var word = _wordParser.Parse(scanner, config);
            if (word is not null)
            {
                words.Add(word);
            }
            else
            {
                break;
            }
        }
        
        if (words.Count == 1)
        {
            if (Node.IsVariable(words[0]))
            {
                return words[0];
            }
        }

        if (IsAssignment(words))
        {
            if (words.Count > 3)
            {
                throw new ParseError($"Unexpected {words[3]["type".ToValue()]} after assignment value.");
            }
            var value = words.Count > 2 ? words[2] : Node.Literal(Value.Empty);
            return Node.Assignment(words[0]["name".ToValue()].ToString(), value);
        }
        
        return words.Count > 0 ? Node.Command(words[0], new ListValue(words.Skip(1))) : null;
    }

    static bool IsAssignment(IReadOnlyList<DictionaryValue> words) =>
        words.Count >= 2
        && Node.IsVariable(words[0]) 
        && Node.IsLiteral(words[1])
        && words[1]["value".ToValue()].ToString() == "=";
}