using Jelly.Parser.Scanning;
using Jelly.Values;

namespace Jelly.Parser;

public class CommentParser : IParser
{
    public DictionaryValue? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsCommentBegin))
        {
            scanner.AdvanceWhile(s => !s.IsCommentEnd);
        }
        return null;
    }
}