namespace Jelly.Commands;

// TODO:  Rename this to ValueGroupCommand, and remove old ValueGroupCommand & VariableGroupCommand.
public class NewValueGroupCommand : GroupCommand
{
    HashSet<string> _mutatorCommands = new(StringComparer.InvariantCultureIgnoreCase);

    public string ValueArgName { get; }

    public string? DefaultSubCommand { get; }

    public NewValueGroupCommand(string name, string valueArgName, string? defaultSubCommand=null) : base(name)
    {
        ValueArgName = valueArgName;
        DefaultSubCommand = defaultSubCommand;
    }

    public void AddMutatorCommand(string name, ICommand command)
    {
        _mutatorCommands.Add(name);
        AddCommand(name, command);
    }

    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        AssertHasValueArg(unevaluatedArgs);
        var subCommandName = GetSubCommandName(unevaluatedArgs, env);
        var isAssignment = IsAssignment(subCommandName);
        if (isAssignment)
        {
            subCommandName = GetAssignmentSubCommandName(env, unevaluatedArgs);
            if (_mutatorCommands.Contains(subCommandName))
            {
                var variableNode = unevaluatedArgs[0].ToNode();
                var variableName = GetVariableName(variableNode, env);
                var variableIndexer = GetVariableIndexer(variableNode, env);
                var value = variableIndexer is not null
                    ? env.CurrentScope.GetVariable(variableName, variableIndexer)
                    : env.CurrentScope.GetVariable(variableName);
                var command = GetCommand(subCommandName);
                var result = command.Invoke(env, new ListValue(new Value[] { Node.Literal(value) }.Concat(unevaluatedArgs.Skip(3))));
                if (variableIndexer is not null)
                {
                    env.CurrentScope.SetVariable(variableName, variableIndexer, result);
                }
                else
                {
                    env.CurrentScope.SetVariable(variableName, result);
                }
                return result;
            }
            throw new ArgError($"Sub-command '{subCommandName}' can not be used in assignment.");
        }
        return InvokeSubCommand(subCommandName, env, unevaluatedArgs);
    }

    static string GetVariableName(DictValue node, IEnv env)
    {
        if (Node.IsVariable(node))
        {
            return node[Keywords.Name].ToString();
        }
        return env.Evaluate(node).ToString();
    }

    static ValueIndexer? GetVariableIndexer(DictValue node, IEnv env)
    {
        if (Node.IsVariable(node) && node.ContainsKey(Keywords.Indexers))
        {
            return ValueIndexer.FromIndexerNodes(node.GetList(Keywords.Indexers), env);
        }
        return null;
    }

    Value InvokeSubCommand(string subCommandName, IEnv env, ListValue unevaluatedArgs)
    {
        var command = GetCommand(subCommandName);
        return command.Invoke(env, new ListValue(unevaluatedArgs.Take(1).Concat(unevaluatedArgs.Skip(2))));
    }

    string GetAssignmentSubCommandName(IEnv env, ListValue unevaluatedArgs)
    {
        AssertHasAssignmentSubCommandName(unevaluatedArgs);
        return env.Evaluate(unevaluatedArgs[2].ToNode()).ToString();
    }

    void AssertHasAssignmentSubCommandName(ListValue unevaluatedArgs)
    {
        if (unevaluatedArgs.Count == 2)
        {
            throw new MissingArgError(_commandName, new[] { new Arg("command") });
        }
    }

    bool IsAssignment(string subCommandName)
    {
        return subCommandName == "=" && _mutatorCommands.Any();
    }

    void AssertHasValueArg(ListValue unevaluatedArgs)
    {
        if (unevaluatedArgs.Count == 0)
        {
            throw new MissingArgError(_commandName, new[] { new Arg(ValueArgName) });
        }
    }

    string GetSubCommandName(ListValue unevaluatedArgs, IEnv env)
    {
        if (unevaluatedArgs.Count == 1)
        {
            if (DefaultSubCommand is not null)
            {
                return DefaultSubCommand;
            }
            else
            {
                throw new MissingArgError(_commandName, new[] { new Arg("command") });
            }
        }
        return env.Evaluate(unevaluatedArgs[1].ToNode()).ToString();
    }
}