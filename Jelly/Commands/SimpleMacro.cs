namespace Jelly.Commands;

public class SimpleMacro : ICommand
{
    public delegate Value MacroDelegate(IEnvironment env, ListValue args);

    readonly MacroDelegate _macro;

    public SimpleMacro(MacroDelegate macro)
    {
        _macro = macro;
    }

    public Value Invoke(IEnvironment env, ListValue args)
    {
        return env.Evaluate(_macro(env, args).ToNode());
    }
}