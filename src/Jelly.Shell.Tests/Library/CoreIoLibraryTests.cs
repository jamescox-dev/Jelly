using Jelly.Shell.Library;

namespace Jelly.Shell.Tests.Library;

[TestFixture]
public class CoreIoLibraryTest
{
    ILibrary _library = null!;

    Env _env = null!;

    FakeReaderWriter _fakeReaderWriter = null!;

    [Test]
    public void TheCorrectCommandsAreDeclaredInTheScope()
    {
        _library.LoadIntoScope(_env.GlobalScope);

        _env.GlobalScope.Invoking(m => m.GetCommand("print")).Should().NotThrow();
        _env.GlobalScope.Invoking(m => m.GetCommand("print...")).Should().NotThrow();
        _env.GlobalScope.Invoking(m => m.GetCommand("input")).Should().NotThrow();
    }

    #region print

    [Test]
    public void ThePrintCommandWritesANewLineWhenNoArgumentsArePassedAndReturnsAnEmptyValue()
    {
        _library.LoadIntoScope(_env.GlobalScope);
        var print = _env.GlobalScope.GetCommand("print");

        var result = print.Invoke(_env, new ListValue());

        result.Should().Be(Value.Empty);
        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp(string.Empty));
    }

    [Test]
    public void ThePrintCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpacesFollowedByANewLine()
    {
        _library.LoadIntoScope(_env.GlobalScope);
        var print = _env.GlobalScope.GetCommand("print");

        print.Invoke(_env, new ListValue(Node.Literal("jello,"), Node.Literal("world")));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("jello, world"));
    }

    #endregion

    #region  print...

    [Test]
    public void ThePrintNoNewLineCommandWritesNothingWhenItHasNoArgumentsAndReturnsAnEmptyValue()
    {
        _library.LoadIntoScope(_env.GlobalScope);
        var print = _env.GlobalScope.GetCommand("print...");

        var result = print.Invoke(_env, new ListValue());

        result.Should().Be(Value.Empty);
        _fakeReaderWriter.IoOps.Should().BeEmpty();
    }

    [Test]
    public void ThePrintNoNewLineCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpaces()
    {
        _library.LoadIntoScope(_env.GlobalScope);
        var print = _env.GlobalScope.GetCommand("print...");

        print.Invoke(_env, new ListValue(Node.Literal("jello,"), Node.Literal("world")));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("jello, world"));
    }

    #endregion

    #region  input

    [Test]
    public void TheInputCommandWritesNothingWhenItHasNoArgumentsAndReturnsAnLineOfInput()
    {
        _fakeReaderWriter.EnqueueInput("Bob");
        _library.LoadIntoScope(_env.GlobalScope);
        var input = _env.GlobalScope.GetCommand("input");

        var result = input.Invoke(_env, new ListValue());

        result.Should().Be("Bob".ToValue());
        _fakeReaderWriter.IoOps.Single().Should().Be(new ReadLineOp("Bob"));
    }

    [Test]
    public void TheInputCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpaces()
    {
        _fakeReaderWriter.EnqueueInput("Bob");
        _library.LoadIntoScope(_env.GlobalScope);
        var print = _env.GlobalScope.GetCommand("input");

        print.Invoke(_env, new ListValue(Node.Literal("what"), Node.Literal("is"), Node.Literal("your"), Node.Literal("name? ")));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("what is your name? "), new ReadLineOp("Bob"));
    }

    #endregion

    [SetUp]
    public void Setup()
    {
        _fakeReaderWriter = new FakeReaderWriter();
        _env = new Env();
        _library = new CoreIoLibrary(_fakeReaderWriter, _fakeReaderWriter);
    }
}