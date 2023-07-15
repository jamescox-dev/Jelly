namespace Jelly.Parser;

public class CommandParser : IParser
{
    readonly WordParser _wordParser;

    public CommandParser(char? terminatingChar = null, ScriptParser? subscriptParser = null)
    {
        _wordParser = new(terminatingChar, subscriptParser);
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        var words = new List<DictionaryValue>();

        while (!scanner.IsCommandSeparator)
        {
            scanner.AdvanceWhile(s => s.IsWordSeparator);

            var word = _wordParser.Parse(scanner);
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
            if (Node.IsVariable(words[0]) || Node.IsExpression(words[0]))
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
            return Node.Assignment(words[0].GetString(Keywords.Name), value);
        }

        return words.Count > 0 ? Node.Command(words[0], new ListValue(words.Skip(1))) : null;
    }

    static bool IsAssignment(IReadOnlyList<DictionaryValue> words) =>
        words.Count >= 2
        && Node.IsVariable(words[0])
        && Node.IsLiteral(words[1])
        && words[1]["value".ToValue()].ToString() == "=";
}