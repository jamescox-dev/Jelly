namespace Jelly.Commands;

public class ValueGroupCommand : GroupCommand
{
    readonly string _valueArgName;
    readonly string? _defaultCommandName;

    public ValueGroupCommand(string commandName, string valueArgName, string? defaultCommandName=null) : base(commandName)
    {
        _valueArgName = valueArgName;
        _defaultCommandName = defaultCommandName;
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        if (unevaluatedArgs.Count == 0)
        {
            throw new MissingArgError(_commandName, new[] { new Arg(_valueArgName) });
        }
        string? commandName = null;
        if (unevaluatedArgs.Count == 1)
        {
            if (_defaultCommandName is not null)
            {
                commandName = _defaultCommandName;
            }
            else
            {
                throw new MissingArgError(_commandName, new[] { new Arg("command") });
            }
        }
        commandName ??= env.Evaluate(unevaluatedArgs[1].ToNode()).ToString();

        var command = GetCommand(commandName);

        return command.Invoke(env, new ListValue(unevaluatedArgs.Take(1).Concat(unevaluatedArgs.Skip(2))));
    }
}