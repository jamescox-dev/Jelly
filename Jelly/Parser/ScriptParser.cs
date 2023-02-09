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

    public DictionaryValue? Parse(Scanner scanner, IParserConfig config)
    {
        var commands = new List<DictionaryValue>();
        var success = true;

        if (_subscriptParser)
        {
            if (scanner.Position < scanner.Source.Length && config.IsScriptCharacter(scanner.Source[scanner.Position]))
            {
                scanner.Advance();
                success = false;
            }
            else
            {
                return null;
            }
        }

        while (scanner.Position < scanner.Source.Length)
        {
            while (scanner.Position < scanner.Source.Length && (config.IsCommandSeparator(scanner.Source[scanner.Position]) || config.IsWordSeparator(scanner.Source[scanner.Position])))
            {
                scanner.Advance();
            }

            if (scanner.Position < scanner.Source.Length)
            {
                if (config.IsScriptEndCharacter(scanner.Source[scanner.Position]))
                {
                    if (_subscriptParser)
                    {
                        scanner.Advance();
                        success = true;
                        break;
                    }
                    else
                    {
                        throw new ParseError($"Unexpected input '{scanner.Source[scanner.Position]}'.");
                    }
                }
                
                var startPosition = scanner.Position;
                var command = CommandParser.Parse(scanner, config);
                if (command is not null)
                {
                    commands.Add(command);
                }
                else if (scanner.Position == startPosition)
                {
                    throw new ParseError($"Unexpected input '{scanner.Source[scanner.Position]}'.");
                }
            }
        }

        if (!success)
        {
            throw new ParseError("Unexpected end-of-file.");
        }

        return Node.Script(commands.ToArray());
    }
}