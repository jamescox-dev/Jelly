namespace Jelly.Shell;

using System.Text.Json;
using System.Collections.Generic;

public class Shell
{
    readonly IReader _reader;
    readonly IWriter _writer;
    readonly IEnvironment _env;
    readonly IEnumerable<ILibrary> _libraries;
    readonly ShellConfig _config;
    readonly IHistoryManager _historyManager;

    public Shell(
        IReader reader,
        IWriter writer,
        IEnvironment env,
        IEnumerable<ILibrary> libraries,
        ShellConfig config,
        IHistoryManager historyManager)
    {
        _reader = reader;
        _writer = writer;
        _env = env;
        _libraries = libraries;
        _config = config;
        _historyManager = historyManager;
    }

    public void Repl()
    {
        WriteWelcomeMessage();

        _historyManager.LoadHistory();

        LoadLibraries();

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

    private void WriteWelcomeMessage()
    {
        _writer.WriteLine(string.Format(_config.WelcomeMessage, JellyInfo.VersionString));
    }

    void LoadLibraries()
    {
        foreach (var library in _libraries)
        {
            library.LoadIntoScope(_env.GlobalScope);
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
                script = _env.Parser.Parse(new Scanner(input));
                if (script is not null)
                {
                    AddHistory(input);
                    // TODO:  This should be a command line option.
                    // Console.WriteLine(JsonSerializer.Serialize(ToClr(script), new JsonSerializerOptions {
                    //     WriteIndented = true
                    // }));
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
        if (!string.IsNullOrWhiteSpace(command))
        {
            _historyManager.AddHistory(command);
        }
    }

    Value Evaluate(DictionaryValue script)
    {
        return _env.Evaluate(script);
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

    public int RunScript(string source)
    {
        LoadLibraries();

        try
        {
            var script = _env.Parser.Parse(new Scanner(source))!;
            _env.Evaluate(script);
            return 0;
        }
        catch (Error error)
        {
            PrintError(error);
            return -1;
        }
    }

    // TODO:  This needs moving, and testing, and should be a command line option...
    // public object? ToClr(Value value)
    // {
    //     return value switch
    //     {
    //         BooleanValue boolean => boolean.ToBool(),
    //         NumberValue number => number.ToDouble(),
    //         StringValue str => str.ToString(),
    //         ListValue list => list.Select(v => ToClr(v)).ToList(),
    //         DictionaryValue dict => new Dictionary<string, object?>(
    //             dict.ToEnumerable().Select(kvp => new KeyValuePair<string, object?>(ToClr(kvp.Key)?.ToString() ?? string.Empty, ToClr(kvp.Value)))),
    //         _ => null
    //     };
    // }
}