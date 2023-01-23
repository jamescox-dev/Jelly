namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

public class VariableParser : IParser
{
    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        if (position < source.Length && config.IsVariableCharacter(source[position]))
        {
            ++position;
            var start = position;
            while (position < source.Length && !config.IsSpecialCharacter(source[position]))
            {
                ++position;
            }
            if (position > start)
            {
                return Node.Variable(source[start..position]);
            }
            else
            {
                throw new ParseError("A variable must have a name.");
            }
        }
        return null;
    }
}