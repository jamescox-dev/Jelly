namespace Jelly.Shell;

using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Parser;
using Jelly.Shell.Io;
using Jelly.Values;

public class Shell
{
    readonly IReader _reader;
    readonly IWriter _writer;
    readonly IScope _globalScope;
    readonly ShellConfig _config;
    readonly IParser _parser;
    readonly IEvaluator _evaluator;

    public Shell(IReader reader, IWriter writer, IScope globalScope, IParser parser, IEvaluator evaluator, ShellConfig config)
    {
        _reader = reader;
        _writer = writer;
        _globalScope = globalScope;
        _parser = parser;
        _evaluator = evaluator;
        _config = config;
    }

    public void Repl()
    {
        for (;;)
        {
            try
            {
                var input = Read();
                PrintResult(Evaluate(input));
            }
            catch (Error error)
            {
                PrintError(error);
            }
            catch (Exception)
            {
                break;
            }
        }
    }

    string Read()
    {
        _writer.Write(_config.Prompt);
        var input = _reader.ReadLine();
        return input;
    }

    Value Evaluate(string source)
    {
        var position = 0;
        var script = _parser.Parse(source, ref position, new DefaultParserConfig());
        if (script is not null)
        {
            return _evaluator.Evaluate(_globalScope, script);
        }
        return Value.Empty;
    }

    void PrintResult(Value result)
    {
        _writer.WriteLine(result.ToString());
    }

    void PrintError(Error error)
    {
        _writer.WriteLine($"ERROR:  {error.Type}:  {error.Message}");
    }

    public static void Main(string[] args)
    {
        new Shell(
            new ReadLineReader(), 
            new ConsoleWriter(),
            new Scope(),
            new ScriptParser(),
            new Evaluator(),
            new ShellConfig()).Repl();
    }
}