namespace Jelly.Library;

public class IoLibrary : ILibrary
{
    static readonly StringValue DstKeyword = new("dst");
    static readonly StringValue FileKeyword = new("file");
    static readonly StringValue PathKeyword = new("path");
    static readonly StringValue PathsKeyword = new("paths");
    static readonly StringValue ReadOnlyKeyword = new("readonly");
    static readonly StringValue RecursiveKeyword = new("recursive");
    static readonly StringValue SrcKeyword = new("src");
    static readonly StringValue TextKeyword = new("text");

    static readonly IArgParser DeleteArgParser = new StandardArgParser(new Arg("path"), new OptArg("recursive", BooleanValue.False));
    static readonly IArgParser FileArgParser = new StandardArgParser(new Arg("file"));
    static readonly IArgParser NoArgParser = new StandardArgParser();
    static readonly IArgParser PathArgParser = new StandardArgParser(new Arg("path"));
    static readonly IArgParser PathsArgParser = new StandardArgParser(new RestArg("paths"));
    static readonly IArgParser LsDirArgParser = new StandardArgParser(new OptArg("path", ".".ToValue()));
    static readonly IArgParser SetReadOnlyArgParser = new StandardArgParser(new Arg("path"), new OptArg("readonly", BooleanValue.True));
    static readonly IArgParser SrcDstArgParser = new StandardArgParser(new Arg("src"), new Arg("dst"));
    static readonly IArgParser WriteAllArgParser = new StandardArgParser(new Arg("file"), new Arg("text"));

    readonly IIoLibraryProvider _provider;

    public IoLibrary(IIoLibraryProvider provider)
    {
        _provider = provider;
    }

    public void LoadIntoScope(IScope scope)
    {
        var ioCmd = new GroupCommand("io");

        ioCmd.AddCommand("exists", new ArgParsedCommand("exists", PathArgParser, ExistsCmd));
        ioCmd.AddCommand("getcwd", new ArgParsedCommand("getcwd", NoArgParser, GetWorkingDirCmd));
        ioCmd.AddCommand("setcwd", new ArgParsedCommand("setcwd", PathArgParser, SetWorkingDirCmd));
        ioCmd.AddCommand("isdir", new ArgParsedCommand("isdir", PathArgParser, IsDirCmd));
        ioCmd.AddCommand("lsdir", new ArgParsedCommand("lsdir", LsDirArgParser, ListDirCmd));
        ioCmd.AddCommand("isreadonly", new ArgParsedCommand("isreadonly", PathArgParser, IsReadOnlyCmd));
        ioCmd.AddCommand("setreadonly", new ArgParsedCommand("setreadonly", SetReadOnlyArgParser, SetReadOnlyCmd));
        ioCmd.AddCommand("readall", new ArgParsedCommand("readall", FileArgParser, ReadAllCmd));
        ioCmd.AddCommand("writeall", new ArgParsedCommand("writeall", WriteAllArgParser, WriteAllCmd));
        ioCmd.AddCommand("del", new ArgParsedCommand("del", DeleteArgParser, DeleteCmd));
        ioCmd.AddCommand("copy", new ArgParsedCommand("copy", SrcDstArgParser, CopyCmd));
        ioCmd.AddCommand("move", new ArgParsedCommand("move", SrcDstArgParser, MoveCmd));
        ioCmd.AddCommand("path", new ArgParsedCommand("path", PathsArgParser, PathCmd));

        scope.DefineCommand("io", ioCmd);
    }

    Value ExistsCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.Exists(args[PathKeyword].ToString()).ToValue();
    }

    Value GetWorkingDirCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.GetWorkingDir().ToValue();
    }

    Value SetWorkingDirCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.SetWorkingDir(args[PathKeyword].ToString());
        return Value.Empty;
    }

    Value IsDirCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.IsDir(args[PathKeyword].ToString()).ToValue();
    }

    Value ListDirCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.ListDir(args[PathKeyword].ToString()).Select(p => p.ToValue()).ToValue();
    }

    Value IsReadOnlyCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.IsReadOnly(args[PathKeyword].ToString()).ToValue();
    }

    Value SetReadOnlyCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.SetReadOnly(args[PathKeyword].ToString(), args[ReadOnlyKeyword].ToBool());
        return Value.Empty;
    }

    Value ReadAllCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.ReadAll(args[FileKeyword].ToString()).ToValue();
    }

    Value WriteAllCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.WriteAll(args[FileKeyword].ToString(), args[TextKeyword].ToString());
        return Value.Empty;
    }

    Value DeleteCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.Delete(args[PathKeyword].ToString(), args[RecursiveKeyword].ToBool());
        return Value.Empty;
    }

    Value MoveCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.Move(args[SrcKeyword].ToString(), args[DstKeyword].ToString());
        return Value.Empty;
    }

    Value CopyCmd(IEnvironment _, DictionaryValue args)
    {
        _provider.Copy(args[SrcKeyword].ToString(), args[DstKeyword].ToString());
        return Value.Empty;
    }

    Value PathCmd(IEnvironment _, DictionaryValue args)
    {
        return _provider.Path(args[PathsKeyword].ToListValue().Select(p => p.ToString())).ToValue();
    }
}