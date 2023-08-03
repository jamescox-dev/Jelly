namespace Jelly.Evaluator;

internal class VariableEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var value = env.CurrentScope.GetVariable(node.GetString(Keywords.Name));

        if (node.TryGetValue(Keywords.Indexers, out var indexersValue))
        {
            var indexers = indexersValue.ToListValue();
            foreach (var indexerValue in indexers)
            {
                var indexer = indexerValue.ToNode();
                if (indexer[Keywords.Type] == Keywords.ListIndexer)
                {
                    var list = value.ToListValue();
                    var index = env.Evaluate(indexer[Keywords.Expression].ToNode()).ToIndexOf(list);
                    value = list[index];
                }
                else
                {
                    var dict = value.ToDictionaryValue();
                    var key = env.Evaluate(indexer[Keywords.Expression].ToNode());
                    value = dict[key];
                }
            }
        }

        return value;
    }
}