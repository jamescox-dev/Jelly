namespace Jelly.Ast.Tests;

[TestFixture]
public class NodeTests
{
    [Test]
    public void ALiteralNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Literal("jello, world".ToValue());

        node.Should().Be(new DictValue(
            "type".ToValue(), "literal".ToValue(),
            "value".ToValue(), "jello, world".ToValue()));
    }

    [Test]
    public void OptionallyTheLiteralNodeCanHaveItSourcePositionSpecified()
    {
        var node = Node.Literal("jello, world".ToValue(), 10, 20);

        node.Should().Be(new DictValue(
            "type".ToValue(), "literal".ToValue(),
            "value".ToValue(), "jello, world".ToValue(),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 10.ToValue(),
                "end".ToValue(), 20.ToValue())));
    }

    [Test]
    public void AVariableNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Variable("answer");

        node.Should().Be(new DictValue(
            "type".ToValue(), "variable".ToValue(),
            "name".ToValue(), "answer".ToValue()));
    }

    [Test]
    public void OptionallyAVariableNodeCanHaveItSourcePositionSpecified()
    {
        var node = Node.Variable("answer", 1, 2);

        node.Should().Be(new DictValue(
            "type".ToValue(), "variable".ToValue(),
            "name".ToValue(), "answer".ToValue(),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 1.ToValue(),
                "end".ToValue(), 2.ToValue())));
    }

    [Test]
    public void OptionallyAVariableNodeCanHaveIndexersSpecified()
    {
        var node = Node.Variable(
            0, 1, "answer",
            Node.ListIndexer(2, 3, Node.Expression()),
            Node.DictIndexer(4, 5, Node.Expression()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "variable".ToValue(),
            "name".ToValue(), "answer".ToValue(),
            "indexers".ToValue(), new ListValue(
                Node.ListIndexer(2, 3, Node.Expression()),
                Node.DictIndexer(4, 5, Node.Expression())),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 0.ToValue(),
                "end".ToValue(), 1.ToValue())));
    }

    [Test]
    public void AListIndexerNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.ListIndexer(1, 2, Node.Expression(3, 4));

        node.Should().Be(new DictValue(
            "type".ToValue(), "listindexer".ToValue(),
            "expression".ToValue(), Node.Expression(3, 4),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 1.ToValue(),
                "end".ToValue(), 2.ToValue())));
    }

    [Test]
    public void ADictIndexerNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.DictIndexer(1, 2, Node.Expression(3, 4));

        node.Should().Be(new DictValue(
            "type".ToValue(), "dictindexer".ToValue(),
            "expression".ToValue(), Node.Expression(3, 4),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 1.ToValue(),
                "end".ToValue(), 2.ToValue())));
    }

    [Test]
    public void ACommandNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var name = Node.Literal("greet".ToValue());
        var args = new ListValue("Vic".ToValue(), "Bob".ToValue());

        var node = Node.Command(name, args);

        node.Should().Be(new DictValue(
            "type".ToValue(), "command".ToValue(),
            "name".ToValue(), name,
            "args".ToValue(), args));
    }

    [Test]
    public void OptionallyACommandNodeCanHaveItSourcePositionSpecified()
    {
        var name = Node.Literal("greet".ToValue());
        var args = new ListValue("Vic".ToValue(), "Bob".ToValue());

        var node = Node.Command(name, args, 3, 141);

        node.Should().Be(new DictValue(
            "type".ToValue(), "command".ToValue(),
            "name".ToValue(), name,
            "args".ToValue(), args,
            "position".ToValue(), new DictValue(
                "start".ToValue(), 3.ToValue(),
                "end".ToValue(), 141.ToValue())));
    }

    [Test]
    public void AScriptNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue());
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue());

        var node = Node.Script(command1, command2);

        node.Should().Be(new DictValue(
            "type".ToValue(), "script".ToValue(),
            "commands".ToValue(), new ListValue(command1, command2)
        ));
    }

    [Test]
    public void OptionallyAScriptNodeCanHaveItSourcePositionSpecified()
    {
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue());
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue());

        var node = Node.Script(100, 200, command1, command2);

        node.Should().Be(new DictValue(
            "type".ToValue(), "script".ToValue(),
            "commands".ToValue(), new ListValue(command1, command2),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 100.ToValue(),
                "end".ToValue(), 200.ToValue())
        ));
    }

    [Test]
    public void ACompositeNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var part1 = Node.Literal("hello".ToValue());
        var part2 = Node.Literal("world".ToValue());

        var node = Node.Composite(part1, part2);

        node.Should().Be(new DictValue(
            "type".ToValue(), "composite".ToValue(),
            "parts".ToValue(), new ListValue(part1, part2)
        ));
    }

    [Test]
    public void OptionallyACompositeNodeCanHaveItSourcePositionSpecified()
    {
        var part1 = Node.Literal("hello".ToValue());
        var part2 = Node.Literal("world".ToValue());

        var node = Node.Composite(100, 120, part1, part2);

        node.Should().Be(new DictValue(
            "type".ToValue(), "composite".ToValue(),
            "parts".ToValue(), new ListValue(part1, part2),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 100.ToValue(),
                "end".ToValue(), 120.ToValue())
        ));
    }

    [Test]
    public void AnAssignmentNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Assignment("username", Node.Literal("Bob".ToValue()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "assignment".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue())
        ));
    }

    [Test]
    public void OptionallyAnAssignmentNodeCanHaveItSourcePositionSpecified()
    {
        var node = Node.Assignment("username", Node.Literal("Bob".ToValue()), 8, 9);

        node.Should().Be(new DictValue(
            "type".ToValue(), "assignment".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue()),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 8.ToValue(),
                "end".ToValue(), 9.ToValue())
        ));
    }

    [Test]
    public void OptionallyAnAssignmentNodeCanHaveIndexersSpecified()
    {
        var node = Node.Assignment(8, 9, "username", Node.Literal("Bob".ToValue()),
            Node.ListIndexer(2, 3, Node.Expression()),
            Node.DictIndexer(4, 5, Node.Expression()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "assignment".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue()),
            "indexers".ToValue(), new ListValue(
                Node.ListIndexer(2, 3, Node.Expression()),
                Node.DictIndexer(4, 5, Node.Expression())),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 8.ToValue(),
                "end".ToValue(), 9.ToValue())
        ));
    }

    [Test]
    public void ADefineVariableCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.DefineVariable("username", Node.Literal("Bob".ToValue()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "defvariable".ToValue(),
            "name".ToValue(), "username".ToValue(),
            "value".ToValue(), Node.Literal("Bob".ToValue())
        ));
    }

    [Test]
    public void AnExpressionNodeCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.Expression(Node.Literal("a".ToValue()), Node.Literal("b".ToValue()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "expression".ToValue(),
            "subexpressions".ToValue(), new ListValue(Node.Literal("a".ToValue()), Node.Literal("b".ToValue()))));
    }

    [Test]
    public void OptionallyAnExpressionNodeCanHaveItSourcePositionSpecified()
    {
        var node = Node.Expression(0, 1, Node.Literal("a".ToValue()), Node.Literal("b".ToValue()));

        node.Should().Be(new DictValue(
            "type".ToValue(), "expression".ToValue(),
            "subexpressions".ToValue(), new ListValue(Node.Literal("a".ToValue()), Node.Literal("b".ToValue())),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 0.ToValue(),
                "end".ToValue(), 1.ToValue())));
    }

    [Test]
    public void ABinaryOperatorCanBeCreatedWithTheCorrectAttributes()
    {
        var node = Node.BinOp("add", Node.Variable("a"), Node.Variable("b"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "binop".ToValue(),
            "op".ToValue(), "add".ToValue(),
            "a".ToValue(), Node.Variable("a"),
            "b".ToValue(), Node.Variable("b")
        ));
    }

    [Test]
    public void OptionallyABinaryOperatorCanHaveItSourcePositionSpecified()
    {
        var node = Node.BinOp(5, 6, "add", Node.Variable("a"), Node.Variable("b"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "binop".ToValue(),
            "op".ToValue(), "add".ToValue(),
            "a".ToValue(), Node.Variable("a"),
            "b".ToValue(), Node.Variable("b"),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 5.ToValue(),
                "end".ToValue(), 6.ToValue())
        ));
    }

    [Test]
    public void AUnaryOperatorCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.UniOp("neg", Node.Variable("a"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "uniop".ToValue(),
            "op".ToValue(), "neg".ToValue(),
            "a".ToValue(), Node.Variable("a")
        ));
    }

    [Test]
    public void OptionallyAUnaryOperatorCanHaveItSourcePositionSpecified()
    {
        var node = Node.UniOp(7, 8, "neg", Node.Variable("a"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "uniop".ToValue(),
            "op".ToValue(), "neg".ToValue(),
            "a".ToValue(), Node.Variable("a"),
            "position".ToValue(), new DictValue(
                "start".ToValue(), 7.ToValue(),
                "end".ToValue(), 8.ToValue())
        ));
    }

    [Test]
    public void AIfCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.If(Node.Literal(true), Node.Variable("thenbody"), Node.Variable("elsebody"));

        node.Should().Be(new DictValue(
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

        node.Should().Be(new DictValue(
            "type".ToValue(), "if".ToValue(),
            "condition".ToValue(), Node.Literal(true),
            "then".ToValue(), Node.Variable("thenbody")
        ));
    }

    [Test]
    public void AWhileCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.While(Node.Literal(true), Node.Variable("dothis"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "while".ToValue(),
            "condition".ToValue(), Node.Literal(true),
            "body".ToValue(), Node.Variable("dothis")
        ));
    }

    [Test]
    public void AScopeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.Scope(Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "scope".ToValue(),
            "body".ToValue(), Node.Literal("body")
        ));
    }

    [Test]
    public void ARaiseNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.Raise(Node.Literal("/return"), Node.Literal("Unexpected return"), Node.Literal("returnValue"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "raise".ToValue(),
            "errortype".ToValue(), Node.Literal("/return"),
            "message".ToValue(), Node.Literal("Unexpected return"),
            "value".ToValue(), Node.Literal("returnValue")
        ));
    }

    [Test]
    public void ATryNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.Try(
            Node.Literal("body"),
            Node.Literal("finallyBody"),
            (Node.Literal("/error1"), Node.Literal("errorBody1")),
            (Node.Literal("/error2"), Node.Literal("errorBody2")));

        node.Should().Be(new DictValue(
            "type".ToValue(), "try".ToValue(),
            "body".ToValue(), Node.Literal("body"),
            "errorhandlers".ToValue(), new ListValue(
                new ListValue(Node.Literal("/error1"), Node.Literal("errorBody1")),
                new ListValue(Node.Literal("/error2"), Node.Literal("errorBody2"))
            ),
            "finally".ToValue(), Node.Literal("finallyBody")
        ));
    }

    [Test]
    public void ATryNodeCanBeCreateWithOutAFinallyBody()
    {
        var node = Node.Try(
            Node.Literal("body"),
            null,
            (Node.Literal("/error1"), Node.Literal("errorBody1")),
            (Node.Literal("/error2"), Node.Literal("errorBody2")));

        node.Should().Be(new DictValue(
            "type".ToValue(), "try".ToValue(),
            "body".ToValue(), Node.Literal("body"),
            "errorhandlers".ToValue(), new ListValue(
                new ListValue(Node.Literal("/error1"), Node.Literal("errorBody1")),
                new ListValue(Node.Literal("/error2"), Node.Literal("errorBody2"))
            )
        ));
    }

    [Test]
    public void ADefineCommandNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.DefineCommand(
            Node.Literal("greet"),
            Node.Script(Node.Command(Node.Literal("sum"), new ListValue(Node.Variable("a"), Node.Variable("b")))),
            new ListValue(Node.Literal("a"), Node.Literal("b")), new ListValue(Node.Literal(1)), Node.Literal("c"));

        node.Should().Be(new DictValue(
            Keywords.Type, Keywords.DefineCommand,
            Keywords.Name, Node.Literal("greet"),
            Keywords.Body, Node.Script(Node.Command(Node.Literal("sum"), new ListValue(Node.Variable("a"), Node.Variable("b")))),
            Keywords.ArgNames, new ListValue(Node.Literal("a"), Node.Literal("b")),
            Keywords.ArgDefaults, new ListValue(Node.Literal(1)),
            Keywords.RestArgName, Node.Literal("c")
        ));
    }

    [Test]
    public void ADefineCommandNodeCanBeCreateWithoutARestParamName()
    {
        var node = Node.DefineCommand(
            Node.Literal("greet"),
            Node.Script(Node.Command(Node.Literal("sum"), new ListValue(Node.Variable("a"), Node.Variable("b")))),
            new ListValue(Node.Literal("a"), Node.Literal("b")), new ListValue(Node.Literal(1)));

        node.Should().Be(new DictValue(
            Keywords.Type, Keywords.DefineCommand,
            Keywords.Name, Node.Literal("greet"),
            Keywords.Body, Node.Script(Node.Command(Node.Literal("sum"), new ListValue(Node.Variable("a"), Node.Variable("b")))),
            Keywords.ArgNames, new ListValue(Node.Literal("a"), Node.Literal("b")),
            Keywords.ArgDefaults, new ListValue(Node.Literal(1))
        ));
    }

    [Test]
    public void AForRangeNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.ForRange(
            Node.Literal("i"),
            Node.Literal(2),
            Node.Literal(10),
            Node.Literal(2),
            Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "forrange".ToValue(),
            "it".ToValue(), Node.Literal("i"),
            "start".ToValue(), Node.Literal(2),
            "end".ToValue(), Node.Literal(10),
            "step".ToValue(), Node.Literal(2),
            "body".ToValue(), Node.Literal("body")
        ));
    }

    [Test]
    public void AForListNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.ForList(
            Node.Literal("i"),
            Node.Literal("j"),
            Node.Literal(new ListValue("a".ToValue(), "b".ToValue())),
            Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "forlist".ToValue(),
            "it_index".ToValue(), Node.Literal("i"),
            "it_value".ToValue(), Node.Literal("j"),
            "list".ToValue(), Node.Literal(new ListValue("a".ToValue(), "b".ToValue())),
            "body".ToValue(), Node.Literal("body")
        ));
    }

    [Test]
    public void AForListNodeCanBeCreateWithoutAnIndexInterator()
    {
        var node = Node.ForList(
            Node.Literal("j"),
            Node.Literal(new ListValue("a".ToValue(), "b".ToValue())),
            Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "forlist".ToValue(),
            "it_value".ToValue(), Node.Literal("j"),
            "list".ToValue(), Node.Literal(new ListValue("a".ToValue(), "b".ToValue())),
            "body".ToValue(), Node.Literal("body")
        ));
    }

    [Test]
    public void AForDictNodeCanBeCreateWithTheCorrectAttributes()
    {
        var node = Node.ForDict(
            Node.Literal("k"),
            Node.Literal("v"),
            Node.Literal(new DictValue("a".ToValue(), "1".ToValue())),
            Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "fordict".ToValue(),
            "it_key".ToValue(), Node.Literal("k"),
            "it_value".ToValue(), Node.Literal("v"),
            "dict".ToValue(),Node.Literal(new DictValue("a".ToValue(), "1".ToValue())),
            "body".ToValue(), Node.Literal("body")
        ));
    }

    [Test]
    public void AForDictNodeCanBeCreateWithoutAValueIterator()
    {
        var node = Node.ForDict(
            Node.Literal("k"),
            Node.Literal(new DictValue("a".ToValue(), "1".ToValue())),
            Node.Literal("body"));

        node.Should().Be(new DictValue(
            "type".ToValue(), "fordict".ToValue(),
            "it_key".ToValue(), Node.Literal("k"),
            "dict".ToValue(),Node.Literal(new DictValue("a".ToValue(), "1".ToValue())),
            "body".ToValue(), Node.Literal("body")
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
        var expression = Node.Expression(literal);

        var isLiteral = Node.IsLiteral(literal);
        var isVariable = Node.IsVariable(variable);
        var isCommand = Node.IsCommand(command);
        var isScript = Node.IsScript(script);
        var isComposite = Node.IsComposite(composite);
        var isAssignment = Node.IsAssignment(assignment);
        var isExpression = Node.IsExpression(expression);

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

    [Test]
    public void ANodeCanBeRepositioned()
    {
        var node = Node.Literal("test", 1, 2);

        var repositionedNode = Node.Reposition(node, 3, 4);

        repositionedNode.Should().Be(Node.Literal("test", 3, 4));
    }

    [Test]
    public void ANodeCanBeRepositionedToTheSamePositionAsAnotherNode()
    {
        var other = Node.Literal("a", 3, 4);
        var node = Node.Literal("test", 1, 2);

        var repositionedNode = Node.Reposition(node, other);

        repositionedNode.Should().Be(Node.Literal("test", 3, 4));
    }

    [Test]
    public void ANodeIsNotRepositionedToTheSamePositionAsAnotherNodeIfTheOtherNodeDoesNotHaveAPosition()
    {
        var other = Node.Literal("a");
        var node = Node.Literal("test", 1, 2);

        var repositionedNode = Node.Reposition(node, other);

        repositionedNode.Should().Be(Node.Literal("test", 1, 2));
    }

    [Test]
    public void ANodeCanBeRepositionedToTheStartOfOneNodeAndTheEndOfAnotherWhenBothNodesHaveAPosition()
    {
        var start = Node.Literal("a", 3, 4);
        var end = Node.Literal("a", 5, 6);
        var node = Node.Literal("test", 1, 2);

        var repositionedNode = Node.Reposition(node, start, end);

        repositionedNode.Should().Be(Node.Literal("test", 3, 6));
    }

    [Test]
    public void ANodeIsNotRepositionedToStartOfOneNodeAndTheEndOfAnotherIfEitherOfTheOtherNodeDoesNotHaveAPosition()
    {
        var start1 = Node.Literal("a");
        var end1 = Node.Literal("a", 5, 6);
        var node1 = Node.Literal("test", 1, 2);

        var repositionedNode1 = Node.Reposition(node1, start1, end1);

        repositionedNode1.Should().Be(Node.Literal("test", 1, 2));

        var start2 = Node.Literal("a", 3, 4);
        var end2 = Node.Literal("a");
        var node2 = Node.Literal("test", 1, 2);

        var repositionedNode2 = Node.Reposition(node2, start2, end2);

        repositionedNode2.Should().Be(Node.Literal("test", 1, 2));
    }

    [Test]
    public void ANodeCanBeRepositionEvenWhenItNeverHadAPosition()
    {
        var node = new DictValue();

        var repositionedNode = Node.Reposition(node, 10, 11);

        repositionedNode.Should().Be(new DictValue(
            "position".ToValue(), new DictValue(
                "start".ToValue(), 10.ToValue(),
                "end".ToValue(), 11.ToValue())));
    }

    [Test]
    public void AVariableNodeCanBeConvertedToALiteralNode()
    {
        var variable = Node.Variable("test");

        var literal = Node.ToLiteralIfVariable(variable);

        literal.Should().Be(Node.Literal("test"));
    }

    [Test]
    public void AVariableNodeCanBeConvertedToALiteralNodeAndThePositionCopied()
    {
        var variable = Node.Variable("test2", 1, 2);

        var literal = Node.ToLiteralIfVariable(variable);

        literal.Should().Be(Node.Literal("test2", 1, 2));
    }

    [Test]
    public void AVariableNodeWithAnIndexerCanNotBeConvertedToALiteralNodeAndAnErrorIsThrown()
    {
        var variable = Node.Variable(1, 2, "test2", Node.ListIndexer(2, 3, Node.Literal(1)), Node.ListIndexer(3, 4, Node.Literal(1)));
        var action = () => Node.ToLiteralIfVariable(variable);

        action.Invoking(a => a())
            .Should().Throw<ArgError>()
            .WithMessage("variable name must not include indexers.")
            .Where(e => e.StartPosition == 2 && e.EndPosition == 4);
    }

    [Test]
    public void ANonVariableNodeIsNotConvertedToALiteralNode()
    {
        var variable = Node.UniOp("-", Node.Literal(1));

        var output = Node.ToLiteralIfVariable(variable);

        output.Should().Be(Node.UniOp("-", Node.Literal(1)));
    }
}