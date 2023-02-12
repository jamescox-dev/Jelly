using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

namespace Jelly.Parser;

public class ExpressionParser : IParser
{
    static readonly WordParser _wordParser = new();

    public DictionaryValue? Parse(Scanner scanner)
    {
        var words = new List<DictionaryValue>();

        if (scanner.AdvanceIf(s => s.IsExpressionBegin))
        {
            DictionaryValue? word = null;
            for (;;)
            {
                scanner.AdvanceWhile(s => s.IsWordSeparator || s.IsCommentEnd);
                word = _wordParser.Parse(scanner);
                if (word is not null)
                {
                    words.Add(word);
                }
                else
                {
                    break;
                }
            }
            scanner.AdvanceWhile(s => s.IsWordSeparator || s.IsCommentEnd);

            if (scanner.AdvanceIf(s => s.IsExpressionEnd))
            {
                return Node.Expression(words.ToArray());
            }
            else
            {
                throw new ParseError("Unexpected end-of-file.");
            }
        }
        return null;
    }
}