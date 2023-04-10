namespace Jelly.Experimental;

using Jelly;
using Jelly.Commands;
using Jelly.Library;
using Jelly.Values;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("cat", new SimpleCommand(CmdCat)); 
        scope.DefineCommand("escape", new SimpleCommand(CmdEscape));
    }

    public Value CmdCat(IScope scope, ListValue args)
    {
        return string.Join("", args).ToValue();
    }

    public Value CmdEscape(IScope scope, ListValue args)
    {
        return args[0].Escape().ToValue();
    }
}
