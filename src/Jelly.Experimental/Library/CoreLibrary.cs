﻿namespace Jelly.Experimental.Library;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("escape", new SimpleCommand(CmdEscape));
    }

    public static Value CmdEscape(ListValue args)
    {
        return args[0].Escape().ToValue();
    }

    // TODO:  set, get, call, def?, var?
}
