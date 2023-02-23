namespace Jelly.Shell;

using Jelly.Ast;
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

    DictionaryValue Read()
    {
        var script = Node.Script();

        var input = "";
        var prompt = _config.Prompt;

        for (;;)
        {
            _writer.Write(prompt);
            var line = _reader.ReadLine();
            input += (input.Length > 0 ? "\n" : "") + line;
            try
            {
                script = _parser.Parse(new Scanner(input));
                if (script is not null)
                {
                    AddHistory(input);
                    return script;
                }
            }
            catch (MissingEndTokenError)
            {
                prompt = _config.ContinuationPrompt;
            }
        }
    }

    void AddHistory(string command)
    {
        if (command.Trim().Length > 0)
        {
            _historyManager.AddHistory(command);
        }
    }

    Value Evaluate(DictionaryValue script)
    {
        return _evaluator.Evaluate(_globalScope, script);
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