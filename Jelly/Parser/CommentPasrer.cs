using Jelly.Values;

namespace Jelly.Parser;

public class CommentParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        if (position < source.Length && config.IsCommentCharacter(source[position]))
        {
            while (position < source.Length && !config.IsCommandSeparator(source[position]))
            {
                ++position;
            }
        }
        return null;
    }
}