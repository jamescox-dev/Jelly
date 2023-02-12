namespace Jelly.Parser;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class ScriptParser : IParser
{
    static readonly CommandParser CommandParser = new();
    
    readonly bool _subscriptParser;

    public ScriptParser(bool subscriptParser=false)
    {
        _subscriptParser = subscriptParser;
    }

    public DictionaryValue? Parse(Scanner scanner)
    {
        var commands = new List<DictionaryValue>();
        var success = true;

        if (_subscriptParser)
        {
            if (scanner.AdvanceIf(s => s.IsScriptBegin))
            {
                success = false;
            }
            else
            {
                return null;
            }
        }

        while (!scanner.IsEof)
        {
            scanner.AdvanceWhile(s => s.IsCommandSeparator || s.IsWordSeparator);

            if (!scanner.IsEof)
            {
                if (scanner.IsScriptEnd)
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
                var command = CommandParser.Parse(scanner);
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