namespace Jelly.Shell.Tests;

using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Parser;
using Jelly.Values;
using Jelly.Shell.Io;

[TestFixture]
public class ShellTests
{
    Shell _shell = null!;

    ShellConfig _config = null!;
    DictionaryValue _expectedParsedScript = null!;

    FakeReaderWriter _fakeReaderWriter = null!;
    Mock<IParser> _mockParser = null!;
    Mock<IEvaluator> _mockEvaluator = null!;
    Mock<IScope> _mockScope = null!;

    [Test]
    public void TheConfiguredPromptIsDisplayed()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteOp("> "));
    }

    [Test]
    public void ALineOfUsersInputIsRead()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteOp("> "), 
            new ReadLineOp("print hello, world")
        );
    }

    [Test]
    public void TheInputIsParsed()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _mockParser.Verify(m => m.Parse("print hello, world", ref It.Ref<int>.IsAny, It.IsAny<DefaultParserConfig>()), Times.Once);
    }

    [Test]
    public void IfTheInputFailsParsedTheParseResultIsNotEvaluated()
    {
        _fakeReaderWriter.EnqueueInput("~~~");

        _shell.Repl();

        _mockEvaluator.Verify(m => m.Evaluate(_mockScope.Object, It.IsAny<DictionaryValue>()), Times.Never);
    }

    [Test]
    public void IfParsingTheInputThrowsAnErrorTheErrorIsWritten()
    {
        _fakeReaderWriter.EnqueueInput("this throws an error");
        _mockParser.Setup(m => m.Parse("this throws an error", ref It.Ref<int>.IsAny, It.IsAny<IParserConfig>()))
            .Throws(Error.Parse("Bad input!"));

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("ERROR:  /error/parse:  Bad input!"));
    }

    [Test]
    public void TheInputIsEvaluated()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _mockEvaluator.Verify(m => m.Evaluate(_mockScope.Object, _expectedParsedScript), Times.Once);
    }

    [Test]
    public void IfEvaluatingTheInputThrowsAnErrorTheErrorIsWritten()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");
        _mockEvaluator.Setup(m => m.Evaluate(It.IsAny<IScope>(), It.IsAny<DictionaryValue>()))
            .Throws(Error.Name("Unknown variable!"));

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("ERROR:  /error/name:  Unknown variable!"));
    }

    [Test]
    public void TheResultIsPrintedToTheScreen()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(new WriteLineOp("the result!"));
    }

    [Test]
    public void TheReplRunsInALoopUntilInterrupetedByAFatalError()
    {
        _fakeReaderWriter.EnqueueInput("print hello, world");
        _fakeReaderWriter.EnqueueInput("print hello, world");

        _shell.Repl();

        _fakeReaderWriter.VerifyIoOpsContains(
            new WriteOp("> "), 
            new ReadLineOp("print hello, world"),
            new WriteLineOp("the result!"),
            new WriteOp("> "), 
            new ReadLineOp("print hello, world"),
            new WriteLineOp("the result!")
        );
    }

    [SetUp]
    public void Setup()
    {
        _config = new ShellConfig();

        _fakeReaderWriter = new FakeReaderWriter();
        _mockParser = new Mock<IParser>();
        _mockEvaluator = new Mock<IEvaluator>();
        _mockScope = new Mock<IScope>();

        _expectedParsedScript = Node.Script(Node.Command(Node.Literal("print".ToValue()), new ListValue(
                Node.Literal("hello,".ToValue()), Node.Literal("world".ToValue())
            )));

        _mockParser.Setup(m => m.Parse("print hello, world", ref It.Ref<int>.IsAny, It.IsAny<DefaultParserConfig>()))
            .Returns(_expectedParsedScript);

        _mockEvaluator.Setup(m => m.Evaluate(_mockScope.Object, _expectedParsedScript)).Returns("the result!".ToValue());

        _shell = new Shell(_fakeReaderWriter, _fakeReaderWriter, _mockScope.Object, _mockParser.Object, _mockEvaluator.Object, _config);
    }

    public class FakeReaderWriter : IReader, IWriter
    {
        readonly Queue<string> _queuedLines = new();

        public IList<IoOp> IoOps { get; set; } = new List<IoOp>();

        public void EnqueueInput(string line) => _queuedLines.Enqueue(line);

        public string ReadLine()
        {
            var line = _queuedLines.Dequeue();
            
            IoOps.Add(new ReadLineOp(line));
            
            return line;
        }

        public void WriteLine(string output) => IoOps.Add(new WriteLineOp(output));
        
        public void Write(string output) => IoOps.Add(new WriteOp(output));

        public bool IoOpsContains(params IoOp[] expected)
        {
            if (IoOps.Count >= expected.Length)
            {
                for (var i = 0; i < IoOps.Count - expected.Length + 1; ++i)
                {
                    if (IoOps.Skip(i).Take(expected.Length).SequenceEqual(expected))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void VerifyIoOpsContains(params IoOp[] expected) => 
            IoOpsContains(expected).Should().BeTrue();
    }

    public abstract record class IoOp(string Data) {}

    public record class ReadLineOp(string Data) : IoOp(Data) {} 

    public record class WriteLineOp(string Data) : IoOp(Data) {} 

    public record class WriteOp(string Data) : IoOp(Data) {} 
}