namespace Jelly.Commands;

public class GroupCommand : CommandBase
{
    protected readonly string _commandName;
    readonly Dictionary<string, ICommand> _subCommands = new(StringComparer.InvariantCultureIgnoreCase);

    public GroupCommand(string commandName)
    {
        _commandName = commandName;
    }

    public void AddCommand(string name, ICommand subCommand)
    {
        _subCommands[name] = subCommand;
    }

    public ICommand GetCommand(string name)
    {
        if (_subCommands.TryGetValue(name, out var command))
        {
            return command;
        }

        throw new NameError($"Unknown sub-command '{name}'.");
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        if (!unevaluatedArgs.Any())
        {
            throw new MissingArgError(_commandName, new[] { new Arg("command") });
        }

        var subCommandName = env.Evaluate(unevaluatedArgs.First().ToNode()).ToString();

        return GetCommand(subCommandName).Invoke(env, new ListValue(unevaluatedArgs.Skip(1)));
    }
}