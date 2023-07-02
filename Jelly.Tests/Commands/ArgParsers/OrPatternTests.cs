namespace Jelly.Commands.ArgParsers.Tests;

[TestFixture]
public class OrPatternTests
{
    [Test]
    public void ConstructingOrPatternWithNoSubPatternsThrowsAnException()
    {
        var action = () => new OrPattern();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void AnOrPatternWithOneSubParserSimplyPassesThePositionAndArgListToTheSubParserAndReturnsTheResult()
    {
        var subPatternResult = new ArgPatternSuccess(0, new Dictionary<string, Value> { { "test", "value".ToValue() } });
        var mockSubPattern = new TestArgPattern(subPatternResult);
        var pattern = new OrPattern(mockSubPattern);

        var result = pattern.Match(10, ListValue.EmptyList);

        mockSubPattern.PositionPassedToMatch.Should().Be(10);
        ((Value?)mockSubPattern.ArgsPassedToMatch).Should().Be(ListValue.EmptyList);
        result.Should().BeEquivalentTo(subPatternResult);
    }

    [Test]
    public void EachSubPatternIsMatchedAndTheFirstToReturnASuccessfulResultIsReturnedAsTheOrPatternsResult()
    {
        var subPattern1 = new KeywordArgPattern("one");
        var subPattern2 = new KeywordArgPattern("two");
        var subPattern3 = new KeywordArgPattern("three");
        var pattern = new OrPattern(subPattern1, subPattern2, subPattern3);

        var result = pattern.Match(1, new ListValue(Value.Empty, Node.Literal("two")));

        result.Should().BeEquivalentTo(new ArgPatternSuccess(2, new Dictionary<string, Value>()));
    }

    [Test]
    public void IfAllSubPatternsReturnAMissingResultTheMissingArgsAreCombinedInTheResultFromTheOrPattern()
    {
        var subPattern1 = new TestArgPattern(new ArgPatternResultMissing(1, new HashSet<Arg>{ new Arg("a") }));
        var subPattern2 = new TestArgPattern(new ArgPatternResultMissing(1, new HashSet<Arg>{ new Arg("b") }));
        var pattern = new OrPattern(subPattern1, subPattern2);

        var result = pattern.Match(1, new ListValue(1.ToValue(), 2.ToValue()));

        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().HaveCount(2);
        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().Contain(new Arg("a"));
        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().Contain(new Arg("b"));
    }

    [Test]
    public void NonMatchingSubPatternsTheMovedThePatternTheFurthestAreCombinedAndReturned()
    {
        var subPattern1 = new TestArgPattern(new ArgPatternResultMissing(1, new HashSet<Arg>{ new Arg("a") }));
        var subPattern2 = new TestArgPattern(new ArgPatternResultMissing(2, new HashSet<Arg>{ new Arg("b") }));
        var subPattern3 = new TestArgPattern(new ArgPatternResultMissing(2, new HashSet<Arg>{ new Arg("c") }));
        var pattern = new OrPattern(subPattern1, subPattern2, subPattern3);

        var result = pattern.Match(1, new ListValue(1.ToValue(), 2.ToValue()));

        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().HaveCount(2);
        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().Contain(new Arg("b"));
        result.Should().BeOfType<ArgPatternResultMissing>().Which.MissingArgs.Should().Contain(new Arg("c"));
    }

    [Test]
    public void NonMatchingSubPatternsThatDoNotReturnMissingResultsAreReturnedIfTheyMoveThePatternAlongFurther()
    {
        var subPattern1 = new TestArgPattern(new ArgPatternResultMissing(1, new HashSet<Arg>{ new Arg("a") }));
        var subPattern2 = new TestArgPattern(new ArgPatternResultMissing(2, new HashSet<Arg>{ new Arg("b") }));
        var subPattern3 = new TestArgPattern(new ArgPatternResultUnexpected(3));
        var pattern = new OrPattern(subPattern1, subPattern2, subPattern3);

        var result = pattern.Match(1, new ListValue(1.ToValue(), 2.ToValue()));

        result.Should().BeOfType<ArgPatternResultUnexpected>().Which.Position.Should().Be(3);
    }
}