namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

public class CommandParser : IParser
{
    readonly WordParser _wordParser = new();

    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var words = new List<DictionaryValue>();

        for (;;)
        {
            while (position < source.Length && config.IsWordSeparator(source[position]))
            {
                ++position;
            }

            var word = _wordParser.Parse(source, ref position, config);
            if (word is not null)
            {
                words.Add(word);
            }
            else
            {
                break;
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
        && MatchWordType(words[0], "variable") 
        && MatchWordType(words[1], "literal") 
        && words[1]["value".ToValue()].ToString() == "=";

    static bool MatchWordType(DictionaryValue word, string type) =>
        word.TryGetValue("type".ToValue(), out var wordType) && wordType.ToString() == type;
}