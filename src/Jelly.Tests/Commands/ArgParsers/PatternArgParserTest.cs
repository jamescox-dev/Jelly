namespace Jelly.Commands.ArgParsers.Tests;

[TestFixture]
public class PatternMatchingArgParserTest
{
    [Test]
    public void TheParserPassesTheArgListToTheArgPatternToParseFromTheBeginning()
    {
        var positionPassedToPattern = -1;
        Value argListPassedToPattern = null!;
        var mockPattern = new Mock<IArgPattern>();
        var args = new ListValue("1".ToValue(), "2".ToValue(), "3".ToValue());
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns<int, ListValue>((p, args) =>
            {
                positionPassedToPattern = p;
                argListPassedToPattern = args;
                return new ArgPatternSuccess(0, new Dictionary<string, Value>());
            });
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Parse("test_command", args);

        positionPassedToPattern.Should().Be(0);
        argListPassedToPattern.Should().Be(args);
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsSuccessTheArgValuesAreReturnedAsADictionaryValue()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(new ArgPatternSuccess(0, new Dictionary<string, Value>()
            {
                { "one", 1.ToValue() },
                { "two", 2.ToValue() }
            }));
        var parser = new PatternArgParser(mockPattern.Object);

        var result = parser.Parse("test_command", ListValue.EmptyList);

        result.Should().Be(new DictValue(
            "one".ToValue(), 1.ToValue(),
            "two".ToValue(), 2.ToValue()
        ));
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsMissingAnErrorIsThrown()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(() => new ArgPatternResultMissing(0, new HashSet<Arg> { new Arg("a") }));
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Invoking(p => p.Parse("need_lots_of_args", ListValue.EmptyList))
            .Should().Throw<MissingArgError>()
            .WithMessage("need_lots_of_args missing argument, expected:  a.");
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsMissingWithMultipleArgumentsAnErrorIsThrown()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(() => new ArgPatternResultMissing(0, new HashSet<Arg> { new Arg("c"), new Arg("a"), new Arg("b") }));
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Invoking(p => p.Parse("test", ListValue.EmptyList))
            .Should().Throw<MissingArgError>()
            .WithMessage("test missing argument, expected:  a, b, or c.");
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsMissingWithMultipleArgumentsAndKeywordsAnErrorIsThrown()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(() => new ArgPatternResultMissing(0, new HashSet<Arg> { new Arg("a"), new Arg("b"), new KwArg("w"), new KwArg("k") }));
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Invoking(p => p.Parse("command", ListValue.EmptyList))
            .Should().Throw<MissingArgError>()
            .WithMessage("command missing argument, expected keyword:  k, or w, or value for:  a, or b.");
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsUnexpectedAnErrorIsThrown()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(() => new ArgPatternResultUnexpected(0, new Arg("arg1")));
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Invoking(p => p.Parse("command", ListValue.EmptyList))
            .Should().Throw<UnexpectedArgError>()
            .WithMessage("command received unexpected argument after arg1.");
    }

    [Test]
    public void IfTheResultOfParsingThePatternIsOfAnUnknownTypeAnErrorIsThrown()
    {
        var mockPattern = new Mock<IArgPattern>();
        mockPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns(() => new ArgPatternResult(0));
        var parser = new PatternArgParser(mockPattern.Object);

        parser.Invoking(p => p.Parse("need_lots_of_args", ListValue.EmptyList))
            .Should().Throw<ArgumentOutOfRangeException>();
    }
}