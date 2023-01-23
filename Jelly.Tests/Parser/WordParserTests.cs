namespace Jelly.Parser.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class WordParserTests
{
    [Test]
    public void ASimpleWordCanBeParsed()
    {
        var parser = new WordParser();
        var source = "jelly";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Literal("jelly".ToValue()));
    }

    [Test]
    public void AVariableCanBeParsed()
    {
        var parser = new WordParser();
        var source = "$jelly";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Variable("jelly"));
    }

    [Test]
    public void AScriptCanBeParsed()
    {
        var parser = new WordParser();
        var source = "{add 1 2}";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(Node.Script(
            Node.Command(Node.Literal("add".ToValue()),
            new ListValue(
                Node.Literal("1".ToValue()),
                Node.Literal("2".ToValue())
            ))
        ));
    }
}