namespace Jelly.Ast.Tests;

using Jelly.Ast;
using Jelly.Values;

[TestFixture]
public class NodeTests
{
    [Test]
    public void ALiteralNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Literal("jello, world".ToValue());

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "literal".ToValue(),
            "value".ToValue(), "jello, world".ToValue()));
    }

    [Test]
    public void AVariableNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Variable("answer");

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "variable".ToValue(),
            "name".ToValue(), "answer".ToValue()));
    }

    [Test]
    public void ACommandNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var name = Node.Literal("greet".ToValue());
        var args = new ListValue("Vic".ToValue(), "Bob".ToValue());

        var node = Node.Command(name, args);

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "command".ToValue(),
            "name".ToValue(), name,
            "args".ToValue(), args));
    }

    [Test]
    public void AScriptNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue());
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue());
        
        var node = Node.Script(command1, command2);

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "script".ToValue(),
            "commands".ToValue(), new ListValue(command1, command2)
        ));
    }

    [Test]
    public void ACompositeNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var part1 = Node.Literal("hello".ToValue());
        var part2 = Node.Literal("world".ToValue());

        var node = Node.Composite(part1, part2);

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "composite".ToValue(),
            "parts".ToValue(), new ListValue(part1, part2)
        ));
    }

    [Test]
    public void AAssignmentNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Assignment("username", Node.Literal("Bob".ToValue()));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "assignment".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue())
        ));
    }

    [Test]
    public void TheTypeOfANodeCanBeChecked()
    {
        var literal = Node.Literal("test".ToValue());
        var variable = Node.Variable("test");
        var command = Node.Command(literal, new ListValue());
        var script = Node.Script();
        var composite = Node.Composite();
        var assignment = Node.Assignment("test", literal);
        
        var isLiteral = Node.IsLiteral(literal);
        var isVariable = Node.IsVariable(variable);
        var isCommand = Node.IsCommand(command);
        var isScript = Node.IsScript(script);
        var isComposite = Node.IsComposite(composite);
        var isAssignment = Node.IsAssignment(assignment);

        isLiteral.Should().BeTrue();
        isVariable.Should().BeTrue();
        isCommand.Should().BeTrue();
        isScript.Should().BeTrue();
        isComposite.Should().BeTrue();
        isAssignment.Should().BeTrue();
    }
}