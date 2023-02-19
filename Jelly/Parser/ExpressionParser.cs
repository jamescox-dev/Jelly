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
        throw new NotImplementedException();
    }
}