namespace Jelly.Shell;

using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Parser;
using Jelly.Parser.Scanning;
using Jelly.Shell.Io;
using Jelly.Values;

public class Shell
{
    readonly IReader _reader;
    readonly IWriter _writer;
    readonly IScope _globalScope;
    readonly IParser _parser;
    readonly IEvaluator _evaluator;
    readonly IEnumerable<ILibrary> _libraries;
    readonly ShellConfig _config;
    
    public Shell(IReader reader, IWriter writer, IScope globalScope, IParser parser, IEvaluator evaluator, IEnumerable<ILibrary> libraries, ShellConfig config)
    {
        _reader = reader;
        _writer = writer;
        _globalScope = globalScope;
        _parser = parser;
        _evaluator = evaluator;
        _libraries = libraries;
        _config = config;
    }

    public void Repl()
    {
        SetupGlobalScope();

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

    void SetupGlobalScope()
    {
        foreach (var library in _libraries)
        {
            library.LoadIntoScope(_globalScope);
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
        var script = _parser.Parse(new Scanner(source));
        if (script is not null)
        {
            return _evaluator.Evaluate(_globalScope, script);
        }
        return Value.Empty;
    }

    void PrintResult(Value result)
    {
        if (result != Value.Empty)
        {
            _writer.WriteLine(result.ToString());
        }
    }

    void PrintError(Error error)
    {
        _writer.WriteLine($"ERROR:  {error.Type}:  {error.Message}");
    }

    public static void Main(string[] args)
    {
        var writer = new ConsoleWriter();
        var reader = new ReadLineReader();
        
        new Shell(
            reader,
            writer,
            new Scope(),
            new ScriptParser(),
            new Evaluator(),
            new ILibrary[] { 
                new Jelly.Experimental.CoreLibrary(),
                new Jelly.Library.CoreLibrary(),
                new Jelly.Shell.Library.CoreIoLibrary(reader, writer)
            },
            new ShellConfig()).Repl();
    }
}