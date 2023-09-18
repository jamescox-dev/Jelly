using Jelly.Parser;

namespace Jelly.Experimental.Library;

public class CollectionsLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("list?", new WrappedCommand(IsList, TypeMarshaller.Shared));
        var listValCmd = new ValueGroupCommand("list", "list", "convert");
        listValCmd.AddCommand("convert", new WrappedCommand(ListConvert, TypeMarshaller.Shared));
        listValCmd.AddCommand("len", new WrappedCommand(ListLen, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("add", new WrappedCommand(ListAdd, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("insert", new WrappedCommand(ListInsert, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("del", new WrappedCommand(ListDel, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("delval", new WrappedCommand(ListDelVal, TypeMarshaller.Shared));
        listValCmd.AddCommand("has?", new WrappedCommand(ListHasValue, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("reverse", new WrappedCommand(ListReverse, TypeMarshaller.Shared));
        listValCmd.AddCommand("get", new WrappedCommand(ListGet, TypeMarshaller.Shared));
        listValCmd.AddMutatorCommand("set", new WrappedCommand(ListSet, TypeMarshaller.Shared));
        scope.DefineCommand("list", listValCmd);

        scope.DefineCommand("dict?", new WrappedCommand(IsList, TypeMarshaller.Shared));
        var dictValCmd = new ValueGroupCommand("dict", "dict", "convert");
        dictValCmd.AddCommand("len", new WrappedCommand(DictLen, TypeMarshaller.Shared));
        dictValCmd.AddCommand("convert", new WrappedCommand(DictConvert, TypeMarshaller.Shared));
        dictValCmd.AddCommand("get", new WrappedCommand(DictGet, TypeMarshaller.Shared));
        dictValCmd.AddMutatorCommand("set", new WrappedCommand(DictSet, TypeMarshaller.Shared));
        dictValCmd.AddMutatorCommand("del", new WrappedCommand(DictDel, TypeMarshaller.Shared));
        dictValCmd.AddCommand("has?", new WrappedCommand(DictHasKey, TypeMarshaller.Shared));
        scope.DefineCommand("dict", dictValCmd);
    }

    static bool IsList(Value value)
    {
        try
        {
            value.ToListValue();
            return true;
        }
        catch (TypeError)
        {
            return false;
        }
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

    static Value ListDel(ListValue list, params Value[] indices)
    {
        return list.RemoveAtRange(indices.Select(i => i.ToIndexOf(list)));
    }

    static Value ListDelVal(ListValue list, params Value[] values)
    {
        return list.RemoveRange(values);
    }

    static Value ListHasValue(ListValue list, Value value)
    {
        return list.Contains(value).ToValue();
    }

    static Value ListReverse(ListValue list)
    {
        return new ListValue(list.Reverse());
    }

    // TODO:  list addall lists...
    // TODO:  list insertall index lists...
    // TODO:  list delall lists...
    // TODO:  list delallval list...
    
    // TODO:  list find value
    // TODO:  list rfind value
    // TODO:  list sort key?     e.g.  list $l sort {list $$ get 1}

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

    // TODO: dict add (key value)...
    // TODO: dict addall dicts...
    // TODO: dict delval values...
    // TODO: dict hasval? values...

    static int DictLen(DictValue dict)
    {
        return dict.Count;
    }

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

    static Value DictSet(DictValue dict, params Value[] items)
    {
        return dict.SetItems(new DictValue(items).ToEnumerable());
    }

    static bool DictHasKey(DictValue dict, Value key)
    {
        return dict.ContainsKey(key);
    }

    static Value DictDel(DictValue dict, params Value[] keys)
    {
        return dict.RemoveRange(keys);
    }
}