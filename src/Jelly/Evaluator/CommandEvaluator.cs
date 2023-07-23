namespace Jelly.Evaluator;

internal class CommandEvaluator : IEvaluator
{
    static readonly StrValue NameKey = new StrValue("name");
    static readonly StrValue ArgsKey = new StrValue("args");

    public Value Evaluate(IEnv env, DictValue node)
    {
        var name = env.Evaluate(node.GetNode(Keywords.Name)).ToString();
        var command = env.CurrentScope.GetCommand(name);
        var args = node.GetList(Keywords.Args);

        return command.Invoke(env, args);
    }
}