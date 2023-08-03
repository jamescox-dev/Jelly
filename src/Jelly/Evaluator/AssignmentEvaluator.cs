namespace Jelly.Evaluator;

internal class AssignmentEvaluator : IEvaluator
{
    public Value Evaluate(IEnv env, DictValue node)
    {
        var name = node.GetString(Keywords.Name);
        var assignmentValue = env.Evaluate(node.GetNode(Keywords.Value));
        var currentValue = assignmentValue;
        if (node.ContainsKey(Keywords.Indexers))
        {
            currentValue = env.CurrentScope.GetVariable(name);
            var previousValues = new Stack<Value>();
            var indexers = node.GetList(Keywords.Indexers).Select(i => EvaluatedIndexer.EvaluateIndexerValue(env, i)).ToArray();

            foreach (var indexer in indexers.Take(indexers.Length - 1))
            {
                previousValues.Push(currentValue);
                currentValue = GetItem(currentValue, indexer);
            }

            currentValue = SetItem(currentValue, indexers.Last(), assignmentValue);

            foreach (var indexer in indexers.Take(indexers.Length - 1).Reverse())
            {
                currentValue = SetItem(previousValues.Pop(), indexer, currentValue);
            }
        }
        env.CurrentScope.SetVariable(name, currentValue);
        return assignmentValue;
    }

    static Value GetItem(Value collection, EvaluatedIndexer indexer)
    {
        if (indexer.IsListIndexer)
        {
            var list = collection.ToListValue();
            var index = indexer.Value.ToIndexOf(list);
            return list[index];
        }
        else
        {
            var dict = collection.ToDictionaryValue();
            var key = indexer.Value;
            return dict[key];
        }
    }

    static Value SetItem(Value collection, EvaluatedIndexer indexer, Value value)
    {
        if (indexer.IsListIndexer)
        {
            var list = collection.ToListValue();
            var index = indexer.Value.ToIndexOf(list);
            return list.SetItem(index, value);
        }
        else
        {
            var dict = collection.ToDictionaryValue();
            var key = indexer.Value;
            return dict.SetItem(key, value);
        }
    }

    record EvaluatedIndexer(bool IsListIndexer, Value Value)
    {
        public static EvaluatedIndexer EvaluateIndexerValue(IEnv env, Value indexer)
        {
            var indexerNode = indexer.ToNode();
            return new(indexerNode[Keywords.Type] == Keywords.ListIndexer,
                    env.Evaluate(indexerNode[Keywords.Expression].ToNode()));
        }
    }
}