namespace Jelly.Parser.Tests;

using Jelly.Values;

[TestFixture]
public class OperatorParserTests
{
    [Test]
    public void IfThereIsAnOperatorAtTheCurrentPositionALiteralNodeIsParsed()
    {
        var parser = new OperatorParser();
        var source = "=";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(1);
        node.Should().Be(Node.Literal("=".ToValue()));
    }

    [Test]
    public void IfTheSourceAtTheCurrentPositionIsNotAOperatorNullIsReturned()
    {
        var parser = new OperatorParser();
        var source = "equals";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        position.Should().Be(0);
        node.Should().BeNull();
    }
}