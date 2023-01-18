namespace Jelly.Parser.Tests;

using Jelly.Values;

[TestFixture]
public class NodeBuilderTests
{
    [Test]
    public void ALiteralNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var builder = new NodeBuilder();

        var node = builder.Literal("jello, world".ToValue());

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "literal".ToValue(),
            "value".ToValue(), "jello, world".ToValue()));
    }

    [Test]
    public void AVariableNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var builder = new NodeBuilder();

        var node = builder.Variable("answer");

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "variable".ToValue(),
            "name".ToValue(), "answer".ToValue()));
    }

    [Test]
    public void ACommandNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var builder = new NodeBuilder();
        var name = builder.Literal("greet".ToValue());
        var args = new ListValue("Vic".ToValue(), "Bob".ToValue());

        var node = builder.Command(name, args);

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "command".ToValue(),
            "name".ToValue(), name,
            "args".ToValue(), args));
    }
}