namespace Jelly.Commands;

public class ArgParsedCommand : CommandBase
{
    public delegate Value CommandDelegate(IEnvironment env, DictionaryValue args);

    public string Name { get; }

    public IArgParser ArgParser { get; }

    readonly CommandDelegate _command;

    public ArgParsedCommand(string name, IArgParser argParser, CommandDelegate command)
    {
        Name = name;
        ArgParser = argParser;
        _command = command;
    }

    public override Value Invoke(IEnvironment env, ListValue unevaluatedArgs)
    {
        var args = EvaluateArgs(env, unevaluatedArgs);
        var parsedArgs = ArgParser.Parse(Name, args);
        return _command(env, parsedArgs);
    }
}