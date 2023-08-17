namespace Jelly.Experimental.Library;

public class CollectionsLibrary : ILibrary
{
    readonly static StrValue DefaultKeyword = new("default");

    readonly static IArgParser ListConvertArgParser = new StandardArgParser(new Arg("list"));
    readonly static IArgParser ListParser = new StandardArgParser(new Arg("list"));
    readonly static IArgParser ListAddParser = new StandardArgParser(new Arg("list"), new RestArg("values"));
    readonly static IArgParser ListInsertParser = new StandardArgParser(new Arg("list"), new Arg("index"), new RestArg("values"));
    readonly static IArgParser ListGetParser = new StandardArgParser(new Arg("list"), new Arg("index"));
    readonly static IArgParser ListSetParser = new StandardArgParser(new Arg("list"), new Arg("index"), new Arg("value"));

    readonly static IArgParser DictConvertArgParser = new StandardArgParser(new Arg("dict"));
    readonly static IArgParser DictGetArgParser = new PatternArgParser(new OrPattern(
        new ExactPattern(new SequenceArgPattern(new SingleArgPattern("dict"), new SingleArgPattern("key"), new SingleArgPattern("default"))),
        new ExactPattern(new SequenceArgPattern(new SingleArgPattern("dict"), new SingleArgPattern("key")))
    ));

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        // TODO:  list?
        var listValCmd = new ValueGroupCommand("list", "list", "convert");
        listValCmd.AddCommand("convert", new ArgParsedCommand("list convert", ListConvertArgParser, ListConvert));
        listValCmd.AddCommand("len", new ArgParsedCommand("list len", ListParser, ListLen));
        listValCmd.AddMutatorCommand("add", new ArgParsedCommand("list add", ListAddParser, ListAdd));
        listValCmd.AddMutatorCommand("reverse", new ArgParsedCommand("list reverse", ListParser, ListReverse));
        listValCmd.AddMutatorCommand("insert", new ArgParsedCommand("list insert", ListInsertParser, ListInsert));
        listValCmd.AddCommand("get", new ArgParsedCommand("list get", ListGetParser, ListGet));
        listValCmd.AddMutatorCommand("set", new ArgParsedCommand("list set", ListSetParser, ListSet));
        scope.DefineCommand("list", listValCmd);

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

    Value ListInsert(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();
        var index = args[Keywords.Index].ToIndexOf(list);
        var values = args[Keywords.Values].ToListValue();

        return list.InsertRange(index, values);
    }

    Value ListReverse(DictValue args)
    {
        var list = args[Keywords.List].ToListValue();

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
        if (args.ContainsKey(DefaultKeyword))
        {
            if (dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return args[DefaultKeyword];
        }
        return dict[key];
    }

    // TODO: dict set (key value)...
}