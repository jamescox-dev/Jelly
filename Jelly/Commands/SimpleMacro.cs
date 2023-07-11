namespace Jelly.Commands;

public class SimpleMacro : ICommand
{
    public delegate Value MacroDelegate(IEnv env, ListValue unevaluatedArgs);

    readonly MacroDelegate _macro;

    public SimpleMacro(MacroDelegate macro)
    {
        _macro = macro;
    }

    public Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        return env.Evaluate(InvokeMacroDelegate(env, unevaluatedArgs));
    }

    internal DictionaryValue InvokeMacroDelegate(IEnv env, ListValue unevaluatedArgs) =>
        _macro(env, unevaluatedArgs).ToNode();
}