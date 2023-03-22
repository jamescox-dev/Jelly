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
    public void AnAssignmentNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Assignment("username", Node.Literal("Bob".ToValue()));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "assignment".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue())
        ));
    }

    [Test]
    public void ADefineVariableCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.DefineVariable("username", Node.Literal("Bob".ToValue()));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "defvariable".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue())
        ));
    }

    [Test]
    public void AnExpressionNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Expression(Node.Literal("a".ToValue()), Node.Literal("b".ToValue()));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "expression".ToValue(),
            "subexpressions".ToValue(), new ListValue(Node.Literal("a".ToValue()), Node.Literal("b".ToValue()))));
    }
    
    [Test]
    public void ABinaryOperatorCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.BinOp("add", Node.Variable("a"), Node.Variable("b"));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "binop".ToValue(),
            "op".ToValue(), "add".ToValue(),
            "a".ToValue(), Node.Variable("a"),
            "b".ToValue(), Node.Variable("b")
        ));
    }

    [Test]
    public void AUnaryOperatorCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.UniOp("neg", Node.Variable("a"));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "uniop".ToValue(),
            "op".ToValue(), "neg".ToValue(),
            "a".ToValue(), Node.Variable("a")
        ));
    }

    [Test]
    public void AIfCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.If(Node.Literal(true), Node.Variable("thenbody"), Node.Variable("elsebody"));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "if".ToValue(),
            "condition".ToValue(), Node.Literal(true),
            "then".ToValue(), Node.Variable("thenbody"),
            "else".ToValue(), Node.Variable("elsebody")
        ));
    }

    [Test]
    public void AIfCanBeCreateWithoutAnElseBody()
    {
        var node = Node.If(Node.Literal(true), Node.Variable("thenbody"));

        node.Should().Be(new DictionaryValue(
            "type".ToValue(), "if".ToValue(),
            "condition".ToValue(), Node.Literal(true),
            "then".ToValue(), Node.Variable("thenbody")
        ));
    }

    [Test]
    public void AWhileCanBeCreateWithTheCorrectAttributes()
    {
        // TODO:  Add while node construction.
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
        var expression = Node.Expression(literal);
        
        var isLiteral = Node.IsLiteral(literal);
        var isVariable = Node.IsVariable(variable);
        var isCommand = Node.IsCommand(command);
        var isScript = Node.IsScript(script);
        var isComposite = Node.IsComposite(composite);
        var isAssignment = Node.IsAssignment(assignment);
        var isExpression = Node.IsExprssion(expression);

        isLiteral.Should().BeTrue();
        isVariable.Should().BeTrue();
        isCommand.Should().BeTrue();
        isScript.Should().BeTrue();
        isComposite.Should().BeTrue();
        isAssignment.Should().BeTrue();
        isExpression.Should().BeTrue();
    }

    [Test]
    public void ALiteralsValueCanBeExtracted()
    {
        var literal = Node.Literal("value".ToValue());

        var value = Node.GetLiteralValue(literal);

        value.Should().Be("value".ToValue());
    }
}