namespace Jelly.Commands.ArgParsers.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class SingleArgParserTests
{
    [Test]
    public void WhenParsingAnEmptyArgumentListAnErrorIsThrown()
    {
        var parser = new SingleArgParser("test1");

        parser.Invoking(p => p.Parse(new ListValue())).Should()
            .Throw<ArgError>().WithMessage("Expected 'test1' argument.");
    }

    [Test]
    public void WhenParsingAnSingleItemArgumentListFromTheBeginningAMatchDictionaryIsReturned()
    {
        var parse = new SingleArgParser("test2");

        var match = parse.Parse(new ListValue("one".ToValue()));

        match.ContainsKey("test2".ToValue()).Should().BeTrue();
        match["test2".ToValue()].Should().Be("one".ToValue());
    }

    [Test]
    public void WhenParsingAnMultiItemArgumentListFromTheBeginningAMatchDictionaryContainingTheFirstArgumentIsReturned()
    {
        var parse = new SingleArgParser("test3");

        var match = parse.Parse(new ListValue("first".ToValue(), "second".ToValue()));

        match.ContainsKey("test3".ToValue()).Should().BeTrue();
        match["test3".ToValue()].Should().Be("first".ToValue());
    }

    [Test]
    public void WhenParsingAnMultiItemArgumentListFromAGivenIndexAMatchDictionaryContainingTheArgumentAtThatIndexIsReturned()
    {
        var parse = new SingleArgParser("test4");

        var match = parse.Parse(new ListValue("first".ToValue(), "second".ToValue()), 1);

        match.ContainsKey("test4".ToValue()).Should().BeTrue();
        match["test4".ToValue()].Should().Be("second".ToValue());
    }

    [Test]
    public void WhenParsingAnMultiItemArgumentListFromAIndexPastEndAMatchDictionaryContainingTheArgumentAtThatIndexIsReturned()
    {
        var parse = new SingleArgParser("test5");

        parse.Invoking(p => p.Parse(new ListValue("first".ToValue(), "second".ToValue()), 2)).Should()
            .Throw<ArgError>().WithMessage("Expected 'test5' argument.");
    }
}