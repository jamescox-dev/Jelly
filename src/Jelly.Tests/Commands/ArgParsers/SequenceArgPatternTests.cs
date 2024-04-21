namespace Jelly.Tests.Commands.ArgParsers;

[TestFixture]
public class SequenceArgPatternTests
{
    [Test]
    public void ConstructingSequencePatternWithNoSubPatternsMatchesThrowsAnException()
    {
        var action = () => new SequenceArgPattern();

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void ASequencePatternWithOnlyOneSubPatternsSimplyPassesTheArgumentsToTheSubParserAndReturnsTheResult()
    {
        var subPatternResult = new ArgPatternSuccess(10, new Dictionary<string, Value>());
        var mockSubPattern = new TestArgPattern(subPatternResult);
        var pattern = new SequenceArgPattern(mockSubPattern);

        var result = pattern.Match(8, new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));

        mockSubPattern.PositionPassedToMatch.Should().Be(8);
        ((Value?)mockSubPattern.ArgsPassedToMatch).Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));
        result.Should().BeEquivalentTo(subPatternResult);
    }

    [Test]
    public void ASequencePatternWithMoreThanOneSubPatternsCombinesTheResultOfEachSubParsersSuccessfullyParsedArgs()
    {
        var subPattern1Result = new ArgPatternSuccess(0, new Dictionary<string, Value>
        {
            { "a", 1.ToValue() }
        });
        var subPattern2Result = new ArgPatternSuccess(0, new Dictionary<string, Value>
        {
            { "b", 2.ToValue() }
        });
        var mockSubPattern1 = new TestArgPattern(subPattern1Result);
        var mockSubPattern2 = new TestArgPattern(subPattern2Result);
        var pattern = new SequenceArgPattern(mockSubPattern1, mockSubPattern2);

        var result = pattern.Match(0, new ListValue("a".ToValue(), "b".ToValue()));

        result.Should().BeOfType<ArgPatternSuccess>().Which.ArgValues.Should().BeEquivalentTo(new Dictionary<string, Value>
        {
            { "a", 1.ToValue() },
            { "b", 2.ToValue() }
        });
    }

    [Test]
    public void TheFirstSubPatternIsMatchedFromTheCurrentPositionAndTheFollowingMatchedFromThePositionOfThePreviousResultTheFinalPositionIsResultOfTheLastSubPattern()
    {
        var mockSubPattern1 = new TestArgPattern(new ArgPatternSuccess(1, new Dictionary<string, Value>()));
        var mockSubPattern2 = new TestArgPattern(new ArgPatternSuccess(3, new Dictionary<string, Value>()));
        var mockSubPattern3 = new TestArgPattern(new ArgPatternSuccess(5, new Dictionary<string, Value>()));
        var pattern = new SequenceArgPattern(mockSubPattern1, mockSubPattern2, mockSubPattern3);

        var result = pattern.Match(0, new ListValue("a".ToValue(), "b".ToValue()));

        mockSubPattern1.PositionPassedToMatch.Should().Be(0);
        mockSubPattern2.PositionPassedToMatch.Should().Be(1);
        mockSubPattern3.PositionPassedToMatch.Should().Be(3);
        result.Position.Should().Be(5);
    }

    [Test]
    public void IfASubPatternFailsToMatchItResultIsReturned()
    {
        var error = new ArgPatternResultUnexpected(10, new Arg("oh"));
        var mockSubPattern1 = new TestArgPattern(new ArgPatternSuccess(1, new Dictionary<string, Value>()));
        var mockSubPattern2 = new TestArgPattern(error);
        var mockSubPattern3 = new TestArgPattern(new ArgPatternSuccess(5, new Dictionary<string, Value>()));
        var pattern = new SequenceArgPattern(mockSubPattern1, mockSubPattern2, mockSubPattern3);

        var result = pattern.Match(0, new ListValue("a".ToValue(), "b".ToValue()));

        result.Should().BeSameAs(error);
    }
}