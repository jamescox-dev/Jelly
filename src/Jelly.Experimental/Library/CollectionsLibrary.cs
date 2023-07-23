namespace Jelly.Experimental.Library;

public class CollectionsLibrary : ILibrary
{
    readonly static IArgParser ListConvertArgParser = new StandardArgParser(new Arg("list"));
    readonly static IArgParser ListLenParser = new StandardArgParser(new Arg("list"));
    readonly static IArgParser ListAddParser = new StandardArgParser(new Arg("list"), new RestArg("values"));
    readonly static IArgParser ListGetParser = new StandardArgParser(new Arg("list"), new Arg("index"));
    readonly static IArgParser ListSetParser = new StandardArgParser(new Arg("list"), new Arg("index"), new Arg("value"));

    readonly static IArgParser DictConvertArgParser = new StandardArgParser(new Arg("dict"));
    readonly static IArgParser DictGetArgParser = new StandardArgParser(new Arg("dict"), new Arg("key"));

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();


        var listValCmd = new ValueGroupCommand("list", "list", "convert");
        var listVarCmd = new VariableGroupCommand("list!");
        listValCmd.AddCommand("convert", new ArgParsedCommand("list convert", ListConvertArgParser, ListConvert));
        listValCmd.AddCommand("len", new ArgParsedCommand("list len", ListLenParser, ListLen));
        var listAddCmd = new ArgParsedCommand("list add", ListAddParser, ListAdd);
        listValCmd.AddCommand("add", listAddCmd);
        listVarCmd.AddCommand("add", listAddCmd);
        listValCmd.AddCommand("get", new ArgParsedCommand("list get", ListGetParser, ListGet));
        var listSetCmd = new ArgParsedCommand("list set", ListSetParser, ListSet);
        listValCmd.AddCommand("set", listSetCmd);
        listVarCmd.AddCommand("set", listSetCmd);
        scope.DefineCommand("list", listValCmd);
        scope.DefineCommand("list!", listVarCmd);

        var dictValCmd = new ValueGroupCommand("dict", "dict", "convert");
        dictValCmd.AddCommand("convert", new ArgParsedCommand("dict convert", DictConvertArgParser, DictConvert));
        dictValCmd.AddCommand("get", new ArgParsedCommand("dict get", DictGetArgParser, DictGet));
        scope.DefineCommand("dict", dictValCmd);
    }

    Value ListConvert(DictValue args)
    {
        return args[Keywords.List].ToListValue();
    }

    Value ListLen(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();
        return list.ToListValue().Count.ToValue();
    }

    Value ListAdd(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();
        var values = args[Keywords.Values].ToListValue();

        return list.AddRange(values);
    }

    // TODO:  list addall lists...
    // TODO:  list insert index value...
    // TODO:  list insertall index lists...
    // TODO:  list del indexes...
    // TODO:  list delval values...
    // TODO:  list find value
    // TODO:  list rfind value
    // TODO:  list sort key?     e.g.  list $l sort {list $$ get 1}
    // TODO:  list reverse

    Value ListGet(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();
        var index = args[Keywords.Index].ToIndexOf(list);

        return list[index];
    }

    Value ListSet(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();
        var index = args[Keywords.Index].ToIndexOf(list);
        var value = args[Keywords.Value];

        return list.SetItem(index, value);
    }

    Value DictConvert(DictValue args)
    {
        return args[Keywords.Dict].ToDictionaryValue();
    }

    // TODO: dict len
    // TODO: dict add (key value)...
    // TODO: dict addall dicts...
    // TODO: dict del keys...
    // TODO: dict delval values...
    // TODO: dict contains keys...
    // TODO: dict containsvalue values...

    Value DictGet(DictValue args)
    {
        var dict = args[Keywords.Dict].ToDictionaryValue();
        var key = args[Keywords.Key];

        return dict[key];
    }

    // TODO: dict set (key value)...
}