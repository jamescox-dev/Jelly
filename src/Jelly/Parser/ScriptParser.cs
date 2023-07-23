namespace Jelly.Parser;

public class ScriptParser : IParser
{
    static CommandParser? TopLevelCommandParser;
    static CommandParser? SubscriptCommandParser;

    readonly CommandParser _commandParser;

    readonly bool _subscriptParser;

    public ScriptParser(bool subscriptParser=false)
    {
        _subscriptParser = subscriptParser;
        if (subscriptParser)
        {
            TopLevelCommandParser = TopLevelCommandParser ?? new(ScannerConfig.Default.ScriptEnd, this);
        }
        else
        {
            SubscriptCommandParser = SubscriptCommandParser ?? new();
        }
        _commandParser = (_subscriptParser ? TopLevelCommandParser : SubscriptCommandParser)!;
    }

    public DictValue? Parse(Scanner scanner)
    {
        var start = scanner.Position;
        var commands = new List<DictValue>();
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
                }

                var startPosition = scanner.Position;
                var command = _commandParser.Parse(scanner);
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
            throw Error.MissingEndToken("Unexpected end-of-file.");
        }

        return Node.Script(start, scanner.Position, commands.ToArray());
    }
}