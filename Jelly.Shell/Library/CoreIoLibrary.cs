namespace Jelly.Shell.Library;

using Jelly;
using Jelly.Commands;
using Jelly.Library;
using Jelly.Shell.Io;
using Jelly.Values;

public class CoreIoLibrary : ILibrary
{
    readonly IReader _reader;
    readonly IWriter _writer;

    public CoreIoLibrary(IReader reader, IWriter writer)
    {
        _reader = reader;
        _writer = writer;
    }

    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("print", new SimpleCommand(CmdPrint));
        scope.DefineCommand("print...", new SimpleCommand(CmdPrintNoNewLine));
        scope.DefineCommand("input", new SimpleCommand(CmdInput));
    }

    public Value CmdPrint(IScope scope, ListValue args)
    {
        _writer.WriteLine(string.Join(" ", args));

        return Value.Empty;
    }

    public Value CmdPrintNoNewLine(IScope scope, ListValue args)
    {
        if (args.Count > 0)
        {
            _writer.Write(string.Join(" ", args));
        }
        
        return Value.Empty;
    }

    public Value CmdInput(IScope scope, ListValue args)
    {
        if (args.Count > 0)
        {
            _writer.Write(string.Join(" ", args));
        }
        
        return _reader.ReadLine().ToValue();
    }
}