namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StringValue NameKey = new StringValue("name");
    static readonly StringValue ArgsKey = new StringValue("args");

    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var name = env.Evaluate(node.GetNode(Keywords.Name)).ToString();
        var command = env.CurrentScope.GetCommand(name);
        var args = node.GetList(Keywords.Args);
        if (CommandArgsShouldBeEvaluated(command))
        {
            args = EvaluateArgs(env, args);
        }

        var returnValue = command.Invoke(env.CurrentScope, args);

        return CommandReturnValueShouldBeEvaluated(command)
            ? env.Evaluate(returnValue.ToNode())
            : returnValue;
    }

    static bool CommandArgsShouldBeEvaluated(ICommand command)
    {
        return (command.EvaluationFlags & EvaluationFlags.Arguments) != 0;
    }

    static bool CommandReturnValueShouldBeEvaluated(ICommand command)
    {
        return (command.EvaluationFlags & EvaluationFlags.ReturnValue) != 0;
    }

    static ListValue EvaluateArgs(IEnvironment env, ListValue args)
    {
        return args.Select(arg => env.Evaluate(arg.ToNode())).ToValue();
    }
}