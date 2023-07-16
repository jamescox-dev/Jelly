namespace Jelly.Parser.Tests;

[TestFixture]
public class WordParserTests
{
    [Test]
    public void ASimpleWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("jelly");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("jelly", 0, 5));
    }

    [Test]
    public void AWordCanHaveAnOptionalTerminatingCharacter()
    {
        var parser = new WordParser('>');
        var scanner = new Scanner("jelly>wobble");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("jelly", 0, 5));
    }

    [Test]
    public void AOperatorCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("=");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("=", 0, 1));
    }

    [Test]
    public void AVariableCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("$jelly");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("jelly", 0, 6));
    }

    [Test]
    public void AScriptCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("{add 1 2}");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("add", 1, 4),
            new ListValue(
                Node.Literal("1", 5, 6),
                Node.Literal("2", 7, 8)
            ))
        ));
    }

    [Test]
    public void ACommentCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("#comment");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(8);
        node.Should().BeNull();
    }

    [Test]
    public void AQuotedWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("'jelly'");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Composite(0, 7, Node.Literal("jelly", 1, 6)));
    }

    [Test]
    public void ANestingWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("[jelly]");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("jelly", 0, 7));
    }

    [Test]
    public void AExpressionCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("(42)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(Node.Literal(42.0.ToValue(), 1, 3)));
    }
}