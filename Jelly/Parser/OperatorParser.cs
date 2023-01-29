using Jelly.Values;

namespace Jelly.Parser;

public class OperatorParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var op = config.GetOperatorAt(source, position);
        if (op is not null)
        {
            position += op.Length;
            return Node.Literal(op.ToValue());
        }
        return null;
    }
}