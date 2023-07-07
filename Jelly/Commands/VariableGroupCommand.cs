namespace Jelly.Commands;

public class VariableGroupCommand : GroupCommand
{
    public VariableGroupCommand(string commandName) : base(commandName)
    {
    }

    public override Value Invoke(IEnvironment env, ListValue unevaluatedArgs)
    {
        if (unevaluatedArgs.Count == 0)
        {
            throw new MissingArgError(_commandName, new[] { new Arg("variable") });
        }
        if (unevaluatedArgs.Count == 1)
        {
            throw new MissingArgError(_commandName, new[] { new Arg("command") });
        }
        var variableName = env.Evaluate(Node.ToLiteralIfVariable(unevaluatedArgs[0].ToNode())).ToString();
        var commandName = env.Evaluate(unevaluatedArgs[1].ToNode()).ToString();

        var command = GetCommand(commandName);
        var value = env.CurrentScope.GetVariable(variableName);

        var result = command.Invoke(env, new ListValue(new[] { Node.Literal(value) }.Concat(unevaluatedArgs.Skip(2))));
        env.CurrentScope.SetVariable(variableName, result);
        return result;
    }
}