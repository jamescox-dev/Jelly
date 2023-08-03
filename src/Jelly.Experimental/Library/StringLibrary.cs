using System.Text;

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
        var typeMarshaller = new TypeMarshaller();

        var strCmd = new ValueGroupCommand("str", "str");
        strCmd.AddCommand("get", new ArgParsedCommand("str get", StrGetArgParser, StrGet));
        strCmd.AddCommand("strip", new ArgParsedCommand("str strip", StrStripArgParser, StrStrip));
        strCmd.AddCommand("join", new ArgParsedCommand("str join", StrJoinArgParser, StrJoin));
        strCmd.AddCommand("joinall", new ArgParsedCommand("str joinall", StrJoinAllArgParser, StrJoinAll));
        strCmd.AddCommand("len", new WrappedCommand(StrLen, typeMarshaller));
        strCmd.AddCommand("repeat", new WrappedCommand(StrRepeat, typeMarshaller));
        strCmd.AddCommand("upper", new WrappedCommand(StrUpper, typeMarshaller));
        strCmd.AddCommand("lower", new WrappedCommand(StrLower, typeMarshaller));
        strCmd.AddCommand("center", new WrappedCommand(StrCenter, typeMarshaller));
        // TODO:  ljust
        // TODO:  rjust
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

    int StrLen(string s)
    {
        return s.Length;
    }

    string StrRepeat(string s, int times)
    {
        var b = new StringBuilder(s.Length * times);
        b.Insert(0, s, times);
        return b.ToString();
    }

    string StrUpper(string str)
    {
        return str.ToUpperInvariant();
    }

    string StrLower(string str)
    {
        return str.ToLowerInvariant();
    }

    string StrCenter(string s, int width, string padding=" ")
    {
        if (padding.Length != 1)
        {
            throw Error.Arg("padding character must be exactly one character long.");
        }
        width = Math.Max(width, s.Length);
        var diff = width - s.Length;
        var padLeft = diff / 2;
        var padRight = width - s.Length - padLeft;
        var b = new StringBuilder(width);
        b.Insert(0, padding, padLeft);
        b.Insert(padLeft, s);
        b.Insert(padLeft + s.Length, padding, padRight);
        return b.ToString();
    }
}