namespace Jelly.Commands;

using Jelly.Values;

public class SimpleMacro : ICommand
{
    public delegate Value MacroDelegate(IScope scope, ListValue args);

    public bool IsMacro => true;

    readonly MacroDelegate _macro;

    public SimpleMacro(MacroDelegate macro) 
    {
        _macro = macro;
    }

    public Value Invoke(IScope scope, ListValue args)
    {
        return _macro(scope, args);
    }
}