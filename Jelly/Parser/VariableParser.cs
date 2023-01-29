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
            int start;
            if (position < source.Length && config.IsVariableDelimiter(source[position]))
            {
                ++position;
                start = position;
                while (position < source.Length && !config.IsVariableEndDelimiter(source[position]))
                {
                    ++position;
                }
                ++position;
                return Node.Variable(source[start..(position - 1)]);
            }
            else
            {
                start = position;
                while (position < source.Length && !(config.IsSpecialCharacter(source[position]) || config.GetOperatorAt(source, position) is not null))
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
        }
        return null;
    }
}