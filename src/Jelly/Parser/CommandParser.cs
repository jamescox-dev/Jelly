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
                var error = new ParseError($"Unexpected {words[3]["type".ToValue()]} after assignment value.");
                error.StartPosition = Node.GetStartPosition(words[3]);
                error.EndPosition = Node.GetEndPosition(words[^1]);
                throw error;
            }
            var variable = words[0];
            var value = words.Count > 2 ? words[2] : Node.Literal(scanner.Position, scanner.Position, Value.Empty);
            var endOfValue = (int)value.ToNode()[Keywords.Position].ToDictValue()[Keywords.End].ToDouble();

            if (variable.ContainsKey(Keywords.Indexers))
            {
                var indexers = variable[Keywords.Indexers].ToListValue().Select(v => v.ToNode()).ToArray();
                return Node.Assignment(start, endOfValue, variable.GetString(Keywords.Name), value, indexers);
            }
            return Node.Assignment(start, endOfValue, variable.GetString(Keywords.Name), value);
        }

        return words.Count > 0 ? BuildCommandNode(words, start) : null;
    }

    static DictValue BuildCommandNode(List<DictValue> words, int start)
    {
        var endOfLastWord = (int)words.Last().ToNode()[Keywords.Position].ToDictValue()[Keywords.End].ToDouble();
        return Node.Command(start, endOfLastWord, words[0], new ListValue(words.Skip(1)));
    }

    static bool IsAssignment(IReadOnlyList<DictValue> words) =>
        words.Count >= 2
        && Node.IsVariable(words[0])
        && Node.IsKeyword(words[1], "=");
}