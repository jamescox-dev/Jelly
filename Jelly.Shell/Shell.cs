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
    readonly IHistoryManager _historyManager;
    
    public Shell(IReader reader, IWriter writer, IScope globalScope, IParser parser, IEvaluator evaluator, IEnumerable<ILibrary> libraries, ShellConfig config, IHistoryManager historyManager)
    {
        _reader = reader;
        _writer = writer;
        _globalScope = globalScope;
        _parser = parser;
        _evaluator = evaluator;
        _libraries = libraries;
        _config = config;
        _historyManager = historyManager;
    }

    public void Repl()
    {
        _historyManager.LoadHistory();

        SetupGlobalScope();

        for (;;)
        {
            try
            {
                var command = Read();
                AddHistory(command);
                PrintResult(Evaluate(command));
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

        _historyManager.SaveHistory();
    }

    void SetupGlobalScope()
    {
        foreach (var library in _libraries)
        {
            library.LoadIntoScope(_globalScope);
        }
    }

    void AddHistory(string command)
    {
        _historyManager.AddHistory(command);
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
}