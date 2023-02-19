namespace Jelly.Shell.Library.Tests;

using Jelly.Library;
using Jelly.Shell.Tests.Helpers;
using Jelly.Values;

[TestFixture]
public class CoreIoLibraryTest
{
    ILibrary _library = null!;

    Scope _scope = null!;

    FakeReaderWriter _fakeReaderWriter = null!;

    [Test]
    public void TheCorrectCommandsAreDeclaredInTheScope()
    {
        _library.LoadIntoScope(_scope);

        _scope.Invoking(m => m.GetCommand("print")).Should().NotThrow();
        _scope.Invoking(m => m.GetCommand("print...")).Should().NotThrow();
        _scope.Invoking(m => m.GetCommand("input")).Should().NotThrow();
    }

    #region print

    [Test]
    public void ThePrintCommandWritesANewLineWhenNoArgumentsArePassedAndReturnsAnEmptyValue()
    {
        _library.LoadIntoScope(_scope);
        var print = _scope.GetCommand("print");

        var result = print.Invoke(_scope, new ListValue());

        result.Should().Be(Value.Empty);
        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp(string.Empty));
    }

    [Test]
    public void ThePrintCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpacesFollowedByANewLine()
    {
        _library.LoadIntoScope(_scope);
        var print = _scope.GetCommand("print");

        print.Invoke(_scope, new ListValue("jello,".ToValue(), "world".ToValue()));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("jello, world"));
    }

    #endregion

    #region  print...

    [Test]
    public void ThePrintNoNewLineCommandWritesNothingWhenItHasNoArgumentsAndReturnsAnEmptyValue()
    {
        _library.LoadIntoScope(_scope);
        var print = _scope.GetCommand("print...");

        var result = print.Invoke(_scope, new ListValue());

        result.Should().Be(Value.Empty);
        _fakeReaderWriter.IoOps.Should().BeEmpty();
    }

    [Test]
    public void ThePrintNoNewLineCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpaces()
    {
        _library.LoadIntoScope(_scope);
        var print = _scope.GetCommand("print...");

        print.Invoke(_scope, new ListValue("jello,".ToValue(), "world".ToValue()));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("jello, world"));
    }

    #endregion

    #region  input

    [Test]
    public void TheInputCommandWritesNothingWhenItHasNoArgumentsAndReturnsAnLineOfInput()
    {
        _fakeReaderWriter.EnqueueInput("Bob");
        _library.LoadIntoScope(_scope);
        var input = _scope.GetCommand("input");

        var result = input.Invoke(_scope, new ListValue());

        result.Should().Be("Bob".ToValue());
        _fakeReaderWriter.IoOps.Single().Should().Be(new ReadLineOp("Bob"));
    }

    [Test]
    public void TheInputCommandWritesEachOfItsArgumentsAsStringsSeparatedBySpaces()
    {
        _fakeReaderWriter.EnqueueInput("Bob");
        _library.LoadIntoScope(_scope);
        var print = _scope.GetCommand("input");

        print.Invoke(_scope, new ListValue("what".ToValue(), "is".ToValue(), "your".ToValue(), "name? ".ToValue()));

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("what is your name? "), new ReadLineOp("Bob"));
    }

    #endregion

    [SetUp]
    public void Setup()
    {
        _fakeReaderWriter = new FakeReaderWriter();
        _scope = new Scope();
        _library = new CoreIoLibrary(_fakeReaderWriter, _fakeReaderWriter);
    }
}