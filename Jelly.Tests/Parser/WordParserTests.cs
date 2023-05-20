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

        node.Should().Be(Node.Literal("jelly".ToValue()));
    }

    [Test]
    public void AWordCanHaveAnOptionalTerminatingCharacter()
    {
        var parser = new WordParser('>');
        var scanner = new Scanner("jelly>wobble");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("jelly".ToValue()));
    }

    [Test]
    public void AOperatorCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("=");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("=".ToValue()));
    }

    [Test]
    public void AVariableCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("$jelly");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Variable("jelly"));
    }

    [Test]
    public void AScriptCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("{add 1 2}");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("add".ToValue()),
            new ListValue(
                Node.Literal("1".ToValue()),
                Node.Literal("2".ToValue())
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

        node.Should().Be(Node.Composite(Node.Literal("jelly".ToValue())));
    }

    [Test]
    public void ANestingWordCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("[jelly]");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Literal("jelly".ToValue()));
    }

    [Test]
    public void AExpressionCanBeParsed()
    {
        var parser = new WordParser();
        var scanner = new Scanner("(42)");

        var node = parser.Parse(scanner);

        node.Should().Be(Node.Expression(Node.Literal(42.0.ToValue())));
    }
}