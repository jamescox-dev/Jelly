namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Values;

public class ScriptParser : IParser
{
    static readonly CommandParser CommandParser = new();
    
    readonly bool _subscriptParser;

    public ScriptParser(bool subscriptParser=false)
    {
        _subscriptParser = subscriptParser;
    }

    public DictionaryValue? Parse(string source, ref int position, IParserConfig config)
    {
        var commands = new List<DictionaryValue>();
        var success = true;

        if (_subscriptParser)
        {
            if (position < source.Length && config.IsScriptCharacter(source[position]))
            {
                ++position;
                success = false;
            }
            else
            {
                return null;
            }
        }

        while (position < source.Length)
        {
            while (position < source.Length && (config.IsCommandSeparator(source[position]) || config.IsWordSeparator(source[position])))
            {
                ++position;
            }

            if (position < source.Length && config.IsScriptEndCharacter(source[position]))
            {
                if (_subscriptParser)
                {
                    ++position;
                    success = true;
                    break;
                }
                else
                {
                    throw new ParseError($"Unexpected input '{source[position]}'.");
                }
            }

            var command = CommandParser.Parse(source, ref position, config);
            if (command is not null)
            {
                commands.Add(command);
            }
        }

        if (!success)
        {
            throw new ParseError("Unexpected end-of-file.");
        }

        return Node.Script(commands.ToArray());
    }
}