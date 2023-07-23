namespace Jelly.Commands;

public class ArgParsedCommand : CommandBase
{
    public delegate Value CommandDelegate(DictValue args);

    public string Name { get; }

    public IArgParser ArgParser { get; }

    readonly CommandDelegate _command;

    public ArgParsedCommand(string name, IArgParser argParser, CommandDelegate command)
    {
        Name = name;
        ArgParser = argParser;
        _command = command;
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        var args = EvaluateArgs(env, unevaluatedArgs);
        var parsedArgs = ArgParser.Parse(Name, args);
        return _command(parsedArgs);
    }
}