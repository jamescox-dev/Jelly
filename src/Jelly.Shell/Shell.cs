namespace Jelly.Shell;

using System.Text.Json;
using System.Collections.Generic;

public class Shell
{
    readonly IReader _reader;
    readonly IWriter _writer;
    readonly IEnv _env;
    readonly IEnumerable<ILibrary> _libraries;
    readonly ShellConfig _config;
    readonly IHistoryManager _historyManager;

    public Shell(
        IReader reader,
        IWriter writer,
        IEnv env,
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
            var source = string.Empty;
            try
            {
                DictValue script;
                (source, script) = Read();
                PrintResult(Evaluate(script));
            }
            catch (Error error)
            {
                PrintError(source, error);
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

    (string, DictValue) Read()
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
                script = _env.Parse(input);
                if (script is not null)
                {
                    AddHistory(input);
                    return (input, script);
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

    Value Evaluate(DictValue script)
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

    void PrintError(string source, Error error)
    {
        var hasPosition = error.StartPosition >= 0;
        if (hasPosition)
        {
            var (ln, col) = source.IndexToLineAndColumn(error.StartPosition);
            _writer.WriteLine($"  ERROR: {error.Type} @ Ln: {ln}, Col: {col}");
            foreach (var underlinedText in source.Underline(error.StartPosition, error.EndPosition))
            {
                _writer.WriteLine($"    {underlinedText.Text}");
                _writer.WriteLine($"    {underlinedText.Underline}");
            }
        }
        else
        {
            _writer.WriteLine($"  ERROR: {error.Type}");
        }
        _writer.WriteLine($"    {error.Message}");
    }

    public int RunScript(string source)
    {
        LoadLibraries();

        try
        {
            var script = _env.Parse(source)!;
            _env.Evaluate(script);
            return 0;
        }
        catch (Error error)
        {
            PrintError(source, error);
            return -1;
        }
    }
}