namespace Jelly.Experimental;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("cat", new SimpleCommand(CmdCat));
        scope.DefineCommand("escape", new SimpleCommand(CmdEscape));
    }

    public Value CmdCat(IEnvironment env, ListValue args)
    {
        return string.Join("", args).ToValue();
    }

    public Value CmdEscape(IEnvironment env, ListValue args)
    {
        return args[0].Escape().ToValue();
    }
}
