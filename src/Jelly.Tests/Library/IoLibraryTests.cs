using Jelly.Library;

namespace Jelly.Tests.Library;

[TestFixture]
public class IoLibraryTests
{
    ILibrary _lib = null!;
    Env _env = null!;
    Mock<IIoLibraryProvider> _mockIoProvider = null!;

    [SetUp]
    public virtual void Setup()
    {
        _mockIoProvider = new();
        _lib = new IoLibrary(_mockIoProvider.Object);
        _env = new();

        _lib.LoadIntoScope(_env.GlobalScope);
    }

    [TestFixture]
    public class WhenLoadedIntoAScope : IoLibraryTests
    {
        [Test]
        public void TheScopeHasTheCorrectCommandsDefined()
        {
            _env.GlobalScope.Invoking(s => s.GetCommand("io")).Should().NotThrow();
            var ioCmd = (GroupCommand)_env.GlobalScope.GetCommand("io");
            ioCmd.Invoking(g => g.GetCommand("exists?")).Should().NotThrow();
            // TODO:  Maybe combine getcwd setcwd into one command.
            ioCmd.Invoking(g => g.GetCommand("getcwd")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("setcwd")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("dir?")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("lsdir")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("readonly?")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("setreadonly")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("readall")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("writeall")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("del")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("copy")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("move")).Should().NotThrow();
            ioCmd.Invoking(g => g.GetCommand("path")).Should().NotThrow();
        }
    }

    [Test]
    public void TheExistsCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        _mockIoProvider.Setup(m => m.Exists(It.IsAny<string>()))
            .Returns((string path) =>
                {
                    pathArg = path;
                    return true;
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("exists?");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/test.txt")));

        pathArg.Should().Be("/test.txt");
        result.Should().Be(BoolValue.True);
    }

    [Test]
    public void TheGetWorkingDirCommandReturnsTheCorrectValue()
    {
        _mockIoProvider.Setup(m => m.GetWorkingDir())
            .Returns("/home");
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("getcwd");

        var result = cmd.Invoke(_env, ListValue.EmptyList);

        result.Should().Be("/home".ToValue());
    }

    [Test]
    public void TheSetWorkingDirCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        _mockIoProvider.Setup(m => m.SetWorkingDir(It.IsAny<string>()))
            .Callback((string path) => pathArg = path);

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("setcwd");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/new/path")));

        pathArg.Should().Be("/new/path");
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheIsDirCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        _mockIoProvider.Setup(m => m.IsDir(It.IsAny<string>()))
            .Returns((string path) =>
                {
                    pathArg = path;
                    return true;
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("dir?");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/dir/")));

        pathArg.Should().Be("/dir/");
        result.Should().Be(BoolValue.True);
    }

    [Test]
    public void TheListDirCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        _mockIoProvider.Setup(m => m.ListDir(It.IsAny<string>()))
            .Returns((string path) =>
                {
                    pathArg = path;
                    return new[] {
                        "one",
                        "two",
                        "three"
                    };
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("lsdir");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/")));

        pathArg.Should().Be("/");
        result.Should().Be(new ListValue("one".ToValue(), "two".ToValue(), "three".ToValue()));
    }

    [Test]
    public void TheIsReadOnlyCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        _mockIoProvider.Setup(m => m.IsReadOnly(It.IsAny<string>()))
            .Returns((string path) =>
                {
                    pathArg = path;
                    return true;
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("readonly?");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/mission")));

        pathArg.Should().Be("/mission");
        result.Should().Be(BoolValue.True);
    }

    [Test]
    public void TheSetReadOnlyCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        var readOnlyArg = false;
        _mockIoProvider.Setup(m => m.SetReadOnly(It.IsAny<string>(), It.IsAny<bool>()))
            .Callback((string path, bool readOnly) =>
            {
                pathArg = path;
                readOnlyArg = readOnly;
            });

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("setreadonly");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/somewhere"), Node.Literal(true)));

        pathArg.Should().Be("/somewhere");
        readOnlyArg.Should().BeTrue();
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheReadAllCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var fileArg = string.Empty;
        _mockIoProvider.Setup(m => m.ReadAll(It.IsAny<string>()))
            .Returns((string file) =>
                {
                    fileArg = file;
                    return "a b c";
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("readall");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("words.txt")));

        fileArg.Should().Be("words.txt");
        result.Should().Be("a b c".ToValue());
    }

    [Test]
    public void TheWriteAllCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var fileArg = string.Empty;
        var textArg = string.Empty;
        _mockIoProvider.Setup(m => m.WriteAll(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string file, string text) =>
            {
                fileArg = file;
                textArg = text;
            });

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("writeall");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("this"), Node.Literal("that")));

        fileArg.Should().Be("this");
        textArg.Should().Be("that");
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheDeleteCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathArg = string.Empty;
        var recursiveArg = false;
        _mockIoProvider.Setup(m => m.Delete(It.IsAny<string>(), It.IsAny<bool>()))
            .Callback((string path, bool recursive) =>
            {
                pathArg = path;
                recursiveArg = recursive;
            });

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("del");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("/removeme"), Node.Literal(true)));

        pathArg.Should().Be("/removeme");
        recursiveArg.Should().BeTrue();
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheMoveCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var srcArg = string.Empty;
        var dstArg = string.Empty;
        _mockIoProvider.Setup(m => m.Move(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string src, string dst) =>
            {
                srcArg = src;
                dstArg = dst;
            });

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("move");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("here"), Node.Literal("there")));

        srcArg.Should().Be("here");
        dstArg.Should().Be("there");
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheCopyCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var srcArg = string.Empty;
        var dstArg = string.Empty;
        _mockIoProvider.Setup(m => m.Copy(It.IsAny<string>(), It.IsAny<string>()))
            .Callback((string src, string dst) =>
            {
                srcArg = src;
                dstArg = dst;
            });

        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("copy");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("here"), Node.Literal("there")));

        srcArg.Should().Be("here");
        dstArg.Should().Be("there");
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void ThePathCommandReceivesTheCorrectArgumentsAndReturnsTheCorrectValue()
    {
        var pathsArg = Array.Empty<string>();
        _mockIoProvider.Setup(m => m.Path(It.IsAny<IEnumerable<string>>()))
            .Returns((IEnumerable<string> paths) =>
                {
                    pathsArg = paths.ToArray();
                    return "/home/jelly";
                });
        var cmd = ((GroupCommand)_env.GlobalScope.GetCommand("io")).GetCommand("path");

        var result = cmd.Invoke(_env, new ListValue(Node.Literal("home"), Node.Literal("jelly")));

        pathsArg.Should().Equal("home", "jelly");
        result.Should().Be("/home/jelly".ToValue());
    }
}