namespace Jelly.Evaluator;

internal class VariableEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node) =>
        env.CurrentScope.GetVariable(node.GetString(Keywords.Name));
}