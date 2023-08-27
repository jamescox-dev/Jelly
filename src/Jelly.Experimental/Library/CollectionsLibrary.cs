namespace Jelly.Experimental.Library;

public class CollectionsLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        // TODO:  list?
        var listValCmd = new ValueGroupCommand("list", "list", "convert");
        listValCmd.AddCommand("convert", new WrappedCommand(ListConvert, TypeMarshaller.Shared));
        listValCmd.AddCommand("len", new WrappedCommand(ListLen, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("add", new WrappedCommand(ListAdd, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("reverse", new WrappedCommand(ListReverse, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("insert", new WrappedCommand(ListInsert, TypeMarshaller.Shared));
        listValCmd.AddCommand("get", new WrappedCommand(ListGet, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("set", new WrappedCommand(ListSet, TypeMarshaller.Shared));
        scope.DefineCommand("list", listValCmd);

        var dictValCmd = new ValueGroupCommand("dict", "dict", "convert");
        dictValCmd.AddCommand("convert", new WrappedCommand(DictConvert, TypeMarshaller.Shared));
        dictValCmd.AddCommand("get", new WrappedCommand(DictGet, TypeMarshaller.Shared));
        scope.DefineCommand("dict", dictValCmd);
    }

    static Value ListConvert(ListValue list)
    {
        return list;
    }

    static Value ListLen(ListValue list)
    {
        return list.Count.ToValue();
    }

    static Value ListAdd(ListValue list, params Value[] values)
    {
        return list.AddRange(values.ToValue());
    }

    static Value ListInsert(ListValue list, NumValue index, params Value[] values)
    {
        return list.InsertRange(index.ToIndexOf(list), new ListValue(values));
    }

    static Value ListReverse(ListValue list)
    {
        return new ListValue(list.Reverse());
    }

    // TODO:  list addall lists...
    // TODO:  list insertall index lists...
    // TODO:  list del indexes...
    // TODO:  list delval values...
    // TODO:  list find value
    // TODO:  list rfind value
    // TODO:  list sort key?     e.g.  list $l sort {list $$ get 1}
    // TODO:  list reverse

    static Value ListGet(ListValue list, NumValue index)
    {
        return list[index.ToIndexOf(list)];
    }

    static Value ListSet(ListValue list, NumValue index, Value value)
    {
        return list.SetItem(index.ToIndexOf(list), value);
    }

    static Value DictConvert(DictValue dict)
    {
        return dict;
    }

    // TODO: dict len
    // TODO: dict add (key value)...
    // TODO: dict addall dicts...
    // TODO: dict del keys...
    // TODO: dict delval values...
    // TODO: dict contains keys...
    // TODO: dict containsvalue values...

    static Value DictGet(DictValue dict, Value key, Value? @default = null)
    {
        if (@default is not null)
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return @default;
        }
        return dict[key];
    }

    // TODO: dict set (key value)...
}