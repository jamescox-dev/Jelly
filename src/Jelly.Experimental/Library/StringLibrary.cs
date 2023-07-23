namespace Jelly.Experimental.Library;

public class StringLibrary : ILibrary
{
    static readonly StrValue ItemsKeyword = new("items");
    static readonly StrValue ListsKeyword = new("lists");

    static readonly IArgParser StrGetArgParser = new StandardArgParser(new Arg("str"), new Arg("index"));
    static readonly IArgParser StrStripArgParser = new StandardArgParser(new Arg("str"));
    static readonly IArgParser StrJoinArgParser = new StandardArgParser(new Arg("str"), new RestArg("items"));
    static readonly IArgParser StrJoinAllArgParser = new StandardArgParser(new Arg("str"), new RestArg("lists"));

    public void LoadIntoScope(IScope scope)
    {
        var strCmd = new ValueGroupCommand("str", "str");
        strCmd.AddCommand("get", new ArgParsedCommand("str get", StrGetArgParser, StrGet));
        strCmd.AddCommand("strip", new ArgParsedCommand("str strip", StrStripArgParser, StrStrip));
        strCmd.AddCommand("join", new ArgParsedCommand("str join", StrJoinArgParser, StrJoin));
        strCmd.AddCommand("joinall", new ArgParsedCommand("str joinall", StrJoinAllArgParser, StrJoinAll));
        scope.DefineCommand("str", strCmd);
    }

    Value StrStrip(DictValue args)
    {
        var str = args[Keywords.Str].ToString();

        return str.Trim().ToValue();
    }

    Value StrGet(DictValue args)
    {
        var str = args[Keywords.Str].ToString();
        var index = args[Keywords.Index].ToIndexOfLength(str.Length);

        if (index >= 0 && index < str.Length)
        {
            return str[index].ToString().ToValue();
        }
        throw new IndexError("index out of bounds.");
    }

    Value StrJoin(DictValue args)
    {
        var str = args[Keywords.Str].ToString();
        var items = args[ItemsKeyword].ToListValue();

        return string.Join(str, items.Select(i => i.ToString())).ToValue();
    }

    Value StrJoinAll(DictValue args)
    {
        var str = args[Keywords.Str].ToString();
        var lists = args[ListsKeyword].ToListValue();

        return string.Join(str, lists.Select(l => string.Join(str, l.ToListValue().Select(i => i.ToString())))).ToValue();
    }
}