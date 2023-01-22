namespace Jelly.Parser;

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
        
        return words.Count > 0 ? NodeBuilder.Shared.Command(words[0], new ListValue(words.Skip(1))) : null;
    }
}