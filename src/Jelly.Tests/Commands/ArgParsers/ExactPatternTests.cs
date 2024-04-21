namespace Jelly.Tests.Commands.ArgParsers;

[TestFixture]
public class ExactPatternTests
{
    [Test]
    public void ThePositionAndArgListIsPassedToTheSubParserAndItsResultReturned()
    {
        var subPatternResult = new ArgPatternSuccess(2, new Dictionary<string, Value>());
        var mockSubPattern = new TestArgPattern(subPatternResult);
        var argList = new ListValue("a".ToValue(), "b".ToValue());
        var pattern = new ExactPattern(mockSubPattern);

        var result = pattern.Match(1, argList);

        mockSubPattern.PositionPassedToMatch.Should().Be(1);
        ((Value?)mockSubPattern.ArgsPassedToMatch).Should().Be(argList);
        result.Should().Be(subPatternResult);
    }

    [Test]
    public void IfTheResultIsSuccessButItsPositionDoesNotReachTheEndOfTheArgListAnUnexpectedResultIsReturned()
    {
        var subPatternResult = new ArgPatternSuccess(1, new Dictionary<string, Value>());
        var mockSubPattern = new TestArgPattern(subPatternResult);
        var argList = new ListValue("a".ToValue(), "b".ToValue());
        var pattern = new ExactPattern(mockSubPattern);

        var result = pattern.Match(1, argList);

        result.Should().BeOfType<ArgPatternResultUnexpected>()
            .Which.Position.Should().Be(1);
        result.Should().BeOfType<ArgPatternResultUnexpected>()
            .Which.LastArg.Should().BeNull();
    }
}