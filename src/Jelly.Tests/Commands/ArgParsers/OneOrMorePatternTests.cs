namespace Jelly.Tests.Commands.ArgParsers;

[TestFixture]
public class OneOrMorePatternTests
{
    [Test]
    public void TheSubPatternIsMatchedAndItResultPlacedInAList()
    {
        var subPattern = new SingleArgPattern("arg");
        var pattern = new OneOrMorePattern(subPattern);

        var result = pattern.Match(1, new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(3);
        result.Should().BeOfType<ArgPatternSuccess>().Which.ArgValues
            .Should().BeEquivalentTo(new Dictionary<string, Value>
            {
                { "arg", new ListValue("b".ToValue(), "c".ToValue()) }
            });
    }

    [Test]
    public void TheSubPatternIsMatchedInALoopMovingTheCurrentPositionToThePositionOfTheLastResultTheFinalResultsPositionIsThatOfTheLastMatch()
    {
        var positionsPassedToSubPattern = new List<int>();
        var mockSubPattern = new Mock<IArgPattern>();
        mockSubPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns<int, ListValue>((position, args) =>
            {
                positionsPassedToSubPattern.Add(position);
                return new ArgPatternSuccess(position + 1, new Dictionary<string, Value>());
            });
        var pattern = new OneOrMorePattern(mockSubPattern.Object);

        var result = pattern.Match(0, new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));

        positionsPassedToSubPattern.Should().Equal(0, 1, 2);
        result.Position.Should().Be(3);
    }

    [Test]
    public void IfThereHaveBeenPreviousMatchesFromTheSubPatternAndTheSubPatternFailsToMatchMatchingStopAndSuccessIsReturned()
    {
        var matches = 0;
        var mockSubPattern = new Mock<IArgPattern>();
        mockSubPattern.Setup(m => m.Match(It.IsAny<int>(), It.IsAny<ListValue>()))
            .Returns<int, ListValue>((position, args) =>
            {
                if (matches < 3)
                {
                    ++matches;
                    return new ArgPatternSuccess(position + 1, new Dictionary<string, Value>());
                }
                return new ArgPatternResultUnexpected(position);
            });
        var pattern = new OneOrMorePattern(mockSubPattern.Object);

        var result = pattern.Match(0, new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue(), "e".ToValue()));

        result.Should().BeOfType<ArgPatternSuccess>().Which.Position.Should().Be(3);
    }

    [Test]
    public void IfThereHaveNotBeenPreviousMatchesFromTheSubPatternAndTheSubPatternFailsToMatchItsResultIsReturned()
    {
        var subPatternResult = new ArgPatternResultUnexpected(0);
        var mockSubPattern = new TestArgPattern(subPatternResult);
        var pattern = new OneOrMorePattern(mockSubPattern);

        var result = pattern.Match(0, new ListValue("hey".ToValue()));

        result.Should().BeSameAs(subPatternResult);
    }
}