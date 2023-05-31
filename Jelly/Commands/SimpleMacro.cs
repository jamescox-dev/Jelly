namespace Jelly.Commands;

public class SimpleMacro : ICommand
{
    public delegate Value MacroDelegate(IEnvironment env, ListValue unevaluatedArgs);

    readonly MacroDelegate _macro;

    public SimpleMacro(MacroDelegate macro)
    {
        _macro = macro;
    }

    public Value Invoke(IEnvironment env, ListValue unevaluatedArgs)
    {
        return env.Evaluate(_macro(env, unevaluatedArgs).ToNode());
    }
}