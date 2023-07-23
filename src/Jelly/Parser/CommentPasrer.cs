


namespace Jelly.Parser;

public class CommentParser : IParser
{
    public DictValue? Parse(Scanner scanner)
    {
        if (scanner.AdvanceIf(s => s.IsCommentBegin))
        {
            scanner.AdvanceWhile(s => !s.IsCommentEnd);
        }
        return null;
    }
}