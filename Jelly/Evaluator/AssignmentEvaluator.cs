namespace Jelly.Evaluator;

internal class AssignmentEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var name = env.Evaluate(node.GetNode(Keywords.Name)).ToString();
        var value = env.Evaluate(node.GetNode(Keywords.Value));
        env.CurrentScope.SetVariable(name, value);
        return value;
    }
}