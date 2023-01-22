namespace Jelly.Parser.Tests;

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

        node.Should().Be(NodeBuilder.Shared.Literal("jelly".ToValue()));
    }

    [Test]
    public void AVariableCanBeParsed()
    {
        var parser = new WordParser();
        var source = "$jelly";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(NodeBuilder.Shared.Variable("jelly"));
    }

    [Test]
    public void AScriptCanBeParsed()
    {
        var parser = new WordParser();
        var source = "{add 1 2}";
        var position = 0;

        var node = parser.Parse(source, ref position, TestParserConfig.Shared);

        node.Should().Be(NodeBuilder.Shared.Script(
            NodeBuilder.Shared.Command(NodeBuilder.Shared.Literal("add".ToValue()),
            new ListValue(
                NodeBuilder.Shared.Literal("1".ToValue()),
                NodeBuilder.Shared.Literal("2".ToValue())
            ))
        ));
    }
}