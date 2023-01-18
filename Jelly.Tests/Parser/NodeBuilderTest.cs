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

    [Test]
    public void AScriptNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var builder = new NodeBuilder();
        var command1 = builder.Command(builder.Literal("command1".ToValue()), new ListValue());
        var command2 = builder.Command(builder.Literal("command2".ToValue()), new ListValue());
        
        var node = builder.Script(command1, command2);

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "script".ToValue(),
            "commands".ToValue(), new ListValue(
                command1, command2
            )
        ));
    }
}