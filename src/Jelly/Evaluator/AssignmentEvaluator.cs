namespace Jelly.Evaluator;

internal class AssignmentEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var name = node.GetString(Keywords.Name);
        var value = env.Evaluate(node.GetNode(Keywords.Value));

        if (node.TryGetValue(Keywords.Indexers, out var indexers))
        {
            var indexer = ValueIndexer.FromIndexerNodes(indexers.ToListValue(), env);
            env.CurrentScope.SetVariable(name, indexer, value);
        }
        else
        {
            env.CurrentScope.SetVariable(name, value);
        }

        return value;
    }
}