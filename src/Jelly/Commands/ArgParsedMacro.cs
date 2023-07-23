namespace Jelly.Commands;

public class ArgParsedMacro : CommandBase
{
    public delegate Value MacroDelegate(IEnv env, DictValue args);

    public string Name { get; }

    public IArgParser ArgParser { get; }

    readonly MacroDelegate _command;

    public ArgParsedMacro(string name, IArgParser argParser, MacroDelegate command)
    {
        Name = name;
        ArgParser = argParser;
        _command = command;
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        var parsedArgs = ArgParser.Parse(Name, unevaluatedArgs);
        return env.Evaluate(_command(env, parsedArgs).ToNode());
    }
}