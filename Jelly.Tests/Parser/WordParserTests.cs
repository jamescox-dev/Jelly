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
}