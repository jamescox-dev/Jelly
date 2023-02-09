using Jelly.Values;

namespace Jelly.Parser;

public class CommentParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        if (scanner.Position < scanner.Source.Length && config.IsCommentCharacter(scanner.Source[scanner.Position]))
        {
            while (scanner.Position < scanner.Source.Length && !config.IsCommandSeparator(scanner.Source[scanner.Position]))
            {
                scanner.Advance();
            }
        }
        return null;
    }
}