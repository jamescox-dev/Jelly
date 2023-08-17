namespace Jelly.Values;

public abstract record class ValueIndexer(Value Index, ValueIndexer? Next=null)
{
    public static ValueIndexer FromIndexerNodes(ListValue indexers, IEnv env)
    {
        ValueIndexer? buildIndexer = null;

        foreach (Value indexerNode in indexers.Reverse())
        {
            buildIndexer = FromNode(indexerNode, buildIndexer, env);
        }

        if (buildIndexer is null)
        {
            throw new ParseError("No indexers in list.");
        }

        return buildIndexer;
    }

    static ValueIndexer FromNode(Value indexerNode, ValueIndexer? prevIndexer, IEnv env)
    {
        if (TryGetIndexerNodeAttributes(indexerNode.ToNode(), out var type, out var expression))
        {
            var index = env.Evaluate(expression);
            if (type == Keywords.ListIndexer)
            {
                return new ListIndexer(index, prevIndexer);
            }
            else if (type == Keywords.DictIndexer)
            {
                return new DictIndexer(index, prevIndexer);
            }
            else
            {
                throw new ParseError($"Unknown indexer type '{type}'.");
            }
        }
        else
        {
            throw new ParseError("Invalid indexer node.");
        }
    }

    static bool TryGetIndexerNodeAttributes(DictValue indexerNode, out Value type, out DictValue expression)
    {
        if (indexerNode.TryGetValue(Keywords.Type, out type)
        && indexerNode.TryGetValue(Keywords.Expression, out var expressionValue))
        {
            expression = expressionValue.ToNode();
            return true;
        }
        expression = DictValue.EmptyDictionary;
        return false;
    }

    public Value GetItemOf(Value collection)
    {
        var value = GetItemOfImpl(collection);
        if (Next is not null)
        {
            return Next.GetItemOf(value);
        }
        return value;
    }

    public Value SetItemOf(Value collection, Value value)
    {
        if (Next is not null)
        {
            value = Next.SetItemOf(GetItemOfImpl(collection), value);
        }
        return SetItemOfImpl(collection, value);
    }

    protected abstract Value GetItemOfImpl(Value collection);

    protected abstract Value SetItemOfImpl(Value collection, Value value);
}

public record class ListIndexer(Value Index, ValueIndexer? Next=null) : ValueIndexer(Index, Next)
{
    protected override Value GetItemOfImpl(Value collection)
    {
        var list = collection.ToListValue();
        return list[Index.ToIndexOf(list)];
    }

    protected override Value SetItemOfImpl(Value collection, Value value)
    {
        var list = collection.ToListValue();
        return list.SetItem(Index.ToIndexOf(list), value);
    }
}

public record class DictIndexer(Value Index, ValueIndexer? Next=null) : ValueIndexer(Index, Next)
{
    protected override Value GetItemOfImpl(Value collection)
    {
        var dict = collection.ToDictValue();
        return dict[Index];
    }

    protected override Value SetItemOfImpl(Value collection, Value value)
    {
        var dict = collection.ToDictValue();
        return dict.SetItem(Index, value);
    }
}