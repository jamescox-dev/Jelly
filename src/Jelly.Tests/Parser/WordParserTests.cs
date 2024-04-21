namespace Jelly.Tests.Parser;

[TestFixture]
public class WordParserTests
{
    [Test]
    public void ASimpleWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("jelly");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 5, "jelly"));
    }

    [Test]
    public void AWordCanHaveAnOptionalTerminatingCharacter()
    {
        var parser = new WordParser('>');
        var scanner = new Scanner("jelly>wobble");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 5, "jelly"));
    }

    [Test]
    public void AOperatorCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("=");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 1, "="));
    }

    [Test]
    public void AVariableCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("$jelly");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable(0, 6, "jelly"));
    }

    [Test]
    public void AScriptCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("{add 1 2}");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(0, 9,
            Node.Command(1, 8, Node.Literal(1, 4, "add"), new ListValue(
                Node.Literal(5, 6, "1"),
                Node.Literal(7, 8, "2")
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

        node.Should().Be(Node.Composite(0, 7, Node.Literal(1, 6, "jelly")));
    }

    [Test]
    public void ANestingWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("[jelly]");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal(0, 7, "jelly"));
    }

    [Test]
    public void AExpressionCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("(42)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(0, 4, Node.Literal(1, 3, 42.0.ToValue())));
    }
}