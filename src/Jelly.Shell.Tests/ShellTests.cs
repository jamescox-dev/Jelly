namespace Jelly.Shell.Tests;

[TestFixture]
public class ShellTests
{
    Shell _shell = null!;

    ShellConfig _config = null!;
    DictValue _expectedParsedScript = null!;

    FakeReaderWriter _fakeReaderWriter = null!;
    Mock<IEnv> _mockEnv = null!;
    Mock<IParser> _mockParser = null!;
    Mock<IScope> _mockScope = null!;
    Mock<ILibrary> _mockLibrary = null!;

    [Test]
    public void TheLibrariesAreEachLoadedIntoTheGlobalScope()
    {
        _shell.Repl();

        _mockLibrary.Verify(m => m.LoadIntoScope(_mockScope.Object), Times.Once);
    }

    [Test]
    public void BeforeEnteringTheReplTheConfiguredWelcomeMessageIsDisplayed()
    {
        _shell.Repl();

        _fakeReaderWriter.IoOps.First().Should().Be(new WriteLineOp(string.Format(_config.WelcomeMessage, JellyInfo.VersionString)));
    }

    [Test]
    public void TheConfiguredPromptIsDisplayed()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("> "));
    }

    [Test]
    public void ALineOfUsersInputIsRead()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteOp("> "),
            new ReadLineOp("print jello, world")
        );
    }

    [Test]
    public void TheInputIsParsed()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _mockEnv.Verify(m => m.Parse("print jello, world"), Times.Once);
    }

    [Test]
    public void IfTheInputFailsParsedTheParseResultIsNotEvaluated()
    {
        _fakeReaderWriter.EnqueueInput("~~~");

        _shell.Repl();

        _mockEnv.Verify(m => m.Evaluate(It.IsAny<DictValue>()), Times.Never);
    }

    [Test]
    public void IfParsingTheInputThrowsAnErrorTheErrorIsWritten()
    {
        _fakeReaderWriter.EnqueueInput("this throws an error");
        _mockEnv.Setup(m => m.Parse("this throws an error"))
            .Throws(Error.Parse("Bad input!"));

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteLineOp("  ERROR: /error/parse/"),
            new WriteLineOp("    Bad input!"));
    }

    [Test]
    public void TheInputIsEvaluated()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _mockEnv.Verify(m => m.Evaluate(_expectedParsedScript), Times.Once);
    }

    [Test]
    public void IfEvaluatingTheInputThrowsAnErrorTheErrorIsWritten()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");
        _mockEnv.Setup(m => m.Evaluate(It.IsAny<DictValue>()))
            .Throws(Error.Name("Unknown variable!"));

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteLineOp("  ERROR: /error/name/"),
            new WriteLineOp("    Unknown variable!"));
    }

    [Test]
    public void IfEvaluatingTheInputThrowsAnErrorWithAStartAndEndPositionTheErrorIsWrittenWithPositionInformation()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world\nprint jello, world");
        _mockEnv.Setup(m => m.Evaluate(It.IsAny<DictValue>()))
            .Throws(new Error("/error/wobbly/", "Wobbly!") { StartPosition = 25, EndPosition = 30 });

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteLineOp("  ERROR: /error/wobbly/ @ Ln: 2, Col: 7"),
            new WriteLineOp("    print jello, world"),
            new WriteLineOp("          ^^^^^"),
            new WriteLineOp("    Wobbly!"));
    }

    [Test]
    public void TheResultIsPrintedToTheScreen()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("the result!"));
    }

    [TestCase("noop1")]
    [TestCase("noop2")]
    [TestCase("noop3")]
    public void IfTheResultIsAnEmptyValueTheResultIsNotPrinted(string input)
    {
        _fakeReaderWriter.EnqueueInput(input);

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsDoesNotContain(new WriteLineOp(""));
    }

    [Test]
    public void TheReplRunsInALoopUntilInterruptedByAFatalError()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteOp("> "),
            new ReadLineOp("print jello, world"),
            new WriteLineOp("the result!"),
            new WriteOp("> "),
            new ReadLineOp("print jello, world"),
            new WriteLineOp("the result!")
        );
    }

    [Test]
    public void WhenACommandIsEnteredItIsAddedToTheHistoryManager()
    {
        _fakeReaderWriter.EnqueueInput("print jello, world");

        _shell.Repl();

        _fakeReaderWriter.RecordedHistory.Should().BeEquivalentTo("print jello, world");
    }

    [Test]
    public void IfTheCommandIsJustWhitespaceItIsNotAddedToTheHistoryManager()
    {
        _fakeReaderWriter.EnqueueInput("  \n\t  ");

        _shell.Repl();

        _fakeReaderWriter.RecordedHistory.Should().BeEmpty();
    }

    [Test]
    public void HistoryIsLoadedBeforeInputIsRead()
    {
        _shell.Repl();

        _fakeReaderWriter.IoOps.Skip(1).Should().StartWith(new LoadHistoryOp());
    }

    [Test]
    public void HistoryIsSavedAfterTheReplExists()
    {
        _shell.Repl();

        _fakeReaderWriter.IoOps.Should().EndWith(new SaveHistoryOp());
    }

    [Test]
    public void IfTheInputThrowsAnMissingEndTokenErrorTheContinuationPromptIsUsedToAddAnotherLineToTheInput()
    {
        _fakeReaderWriter.EnqueueInput("print 'jello,");
        _fakeReaderWriter.EnqueueInput("world'");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteOp("> "),
            new ReadLineOp("print 'jello,"),
            new WriteOp(". "),
            new ReadLineOp("world'"),
            new WriteLineOp("the result!")
        );
    }

    [Test]
    public void BeforeRunningAScriptTheLibrariesAreEachLoadedIntoTheGlobalScope()
    {
        _shell.RunScript("print jello, world");

        _mockLibrary.Verify(m => m.LoadIntoScope(_mockScope.Object), Times.Once);
    }

    [Test]
    public void RunningAScriptReturnsZeroOnSuccess()
    {
        var result = _shell.RunScript("print jello, world");

        result.Should().Be(0);
    }

    [Test]
    public void RunningAScriptFirstParseTheSource()
    {
        _shell.RunScript("print jello, world");

        _mockEnv.Verify(m => m.Parse("print jello, world"), Times.Once);
    }

    [Test]
    public void RunningAScriptEvaluatesTheParsedSource()
    {
        _shell.RunScript("print jello, world");

        _mockEnv.Verify(m => m.Evaluate(It.IsAny<DictValue>()), Times.Once);
    }

    [Test]
    public void IfAErrorIsThrownItIsDisplayedToTheUserAndAnErrorCodeIsReturned()
    {
        _mockEnv.Setup(m => m.Evaluate(It.IsAny<DictValue>()))
            .Throws(Error.Name("Unknown variable!"));

        var result = _shell.RunScript("print jello, world");

        result.Should().Be(-1);
        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteLineOp("  ERROR: /error/name/"),
            new WriteLineOp("    Unknown variable!"));
    }

    [SetUp]
    public void Setup()
    {
        _config = new ShellConfig();

        _mockEnv = new Mock<IEnv>();
        _fakeReaderWriter = new FakeReaderWriter();
        _mockParser = new Mock<IParser>();
        _mockScope = new Mock<IScope>();
        _mockLibrary = new Mock<ILibrary>();

        _mockEnv.SetupGet(m => m.GlobalScope).Returns(_mockScope.Object);
        _mockEnv.SetupGet(m => m.Parser).Returns(_mockParser.Object);

        _expectedParsedScript = Node.Script(Node.Command(Node.Literal("print".ToValue()), new ListValue(
                Node.Literal("jello,".ToValue()), Node.Literal("world".ToValue())
            )));

        _mockEnv.Setup(m => m.Parse("print jello, world"))
            .Returns(_expectedParsedScript);

        _mockEnv.Setup(m => m.Parse("print 'jello,\nworld'"))
            .Returns(_expectedParsedScript);

        _mockEnv.Setup(m => m.Parse("print hello, world\nprint jello, world"))
            .Returns(_expectedParsedScript);

        _mockEnv.Setup(m => m.Parse("print 'jello,"))
            .Throws(Error.MissingEndToken("Oh!  No!"));

        _mockEnv.Setup(m => m.Parse("noop1"))
            .Returns(new DictValue("noop1".ToValue()));
        _mockEnv.Setup(m => m.Parse("noop2"))
            .Returns(new DictValue("noop2".ToValue()));
        _mockEnv.Setup(m => m.Parse("noop3"))
            .Returns(new DictValue("noop3".ToValue()));

        _mockEnv.Setup(m => m.Evaluate(_expectedParsedScript)).Returns("the result!".ToValue());
        _mockEnv.Setup(m => m.Evaluate(new DictValue("noop1".ToValue()))).Returns(Value.Empty);
        _mockEnv.Setup(m => m.Evaluate(new DictValue("noop2".ToValue()))).Returns(ListValue.EmptyList);
        _mockEnv.Setup(m => m.Evaluate(new DictValue("noop3".ToValue()))).Returns(DictValue.EmptyDictionary);

        _shell = new Shell(
            _fakeReaderWriter,
            _fakeReaderWriter,
            _mockEnv.Object,
            new ILibrary[] {
                _mockLibrary.Object
            },
            _config,
            _fakeReaderWriter
        );
    }
}