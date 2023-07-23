namespace Jelly.Parser;

public class CommandParser : IParser
{
    readonly WordParser _wordParser;

    public CommandParser(char? terminatingChar = null, ScriptParser? subscriptParser = null)
    {
        _wordParser = new(terminatingChar, subscriptParser);
    }

    public DictValue? Parse(Scanner scanner)
    {
        var words = new List<DictValue>();

        var start = scanner.Position;
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
            var value = words.Count > 2 ? words[2] : Node.Literal(Value.Empty, scanner.Position, scanner.Position);
            var endOfValue = (int)value.ToNode()[Keywords.Position].ToDictionaryValue()[Keywords.End].ToDouble();
            return Node.Assignment(words[0].GetString(Keywords.Name), value, start, endOfValue);
        }

        return words.Count > 0 ? BuildCommandNode(words, start) : null;
    }

    static DictValue BuildCommandNode(List<DictValue> words, int start)
    {
        var endOfLastWord = (int)words.Last().ToNode()[Keywords.Position].ToDictionaryValue()[Keywords.End].ToDouble();
        return Node.Command(words[0], new ListValue(words.Skip(1)), start, endOfLastWord);
    }

    static bool IsAssignment(IReadOnlyList<DictValue> words) =>
        words.Count >= 2
        && Node.IsVariable(words[0])
        && Node.IsLiteral(words[1])
        && words[1]["value".ToValue()].ToString() == "=";
}