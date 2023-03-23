namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Parser.Scanning;
using Jelly.Values;

[TestFixture]
public class OperatorParserTests
{
    [Test]
    public void IfThereIsAnOperatorAtTheCurrentPositionALiteralNodeIsParsed()
    {
        var parser = new OperatorParser();
        var scanner = new Scanner("<=");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        node.Should().Be(Node.Literal("<=".ToValue()));
    }

    [Test]
    public void IfTheSourceAtTheCurrentPositionIsNotAOperatorNullIsReturned()
    {
        var parser = new OperatorParser();
        var scanner = new Scanner("equals");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        node.Should().BeNull();
    }
}