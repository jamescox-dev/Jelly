namespace Jelly.Evaluator;

internal class VariableEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var name = node.GetString(Keywords.Name);

        if (node.TryGetValue(Keywords.Indexers, out var indexers))
        {
            var indexer = ValueIndexer.FromIndexerNodes(indexers.ToListValue(), env);
            return env.CurrentScope.GetVariable(name, indexer);
        }

        return env.CurrentScope.GetVariable(name);
    }
}