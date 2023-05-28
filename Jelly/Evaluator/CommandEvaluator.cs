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

        return command.Invoke(env, args);
    }
}