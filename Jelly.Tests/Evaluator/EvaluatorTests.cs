namespace Jelly.Evaluator.Tests;

[TestFixture]
public class EvaluatorTests
{
    [Test]
    public void TheEvaluatorCanEvaluateALiteralNode()
    {
        IEvaluator evaluator = new Evaluator();
        var node = Node.Literal("Hi".ToValue());

        var result = evaluator.Evaluate(new Mock<IScope>().Object, node);

        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateACompositeNode()
    {
        IEvaluator evaluator = new Evaluator();
        var node = Node.Composite(Node.Literal("Hi".ToValue()));

        var result = evaluator.Evaluate(new Mock<IScope>().Object, node);

        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAVariableNode()
    {
        IEvaluator evaluator = new Evaluator();
        Mock<IScope> scope = new Mock<IScope>();
        scope.Setup(m => m.GetVariable("Name")).Returns("Bill".ToValue());
        var node = Node.Variable("Name");

        var result = evaluator.Evaluate(scope.Object, node);

        result.Should().Be("Bill".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateACommandNode()
    {
        IEvaluator evaluator = new Evaluator();
        Mock<IScope> scope = new Mock<IScope>();
        Mock<ICommand> command = new Mock<ICommand>();
        command.SetupGet(m => m.EvaluationFlags).Returns(EvaluationFlags.Arguments);
        scope.Setup(m => m.GetCommand("Foo")).Returns(command.Object);
        var node = Node.Command(Node.Literal("Foo".ToValue()), new ListValue());

        var result = evaluator.Evaluate(scope.Object, node);

        command.Verify(m => m.Invoke(scope.Object, new ListValue()), Times.Once);
    }

    [Test]
    public void TheEvaluatorCanEvaluateAScriptNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var command = new Mock<ICommand>();
        command.SetupGet(m => m.EvaluationFlags).Returns(EvaluationFlags.Arguments);
        command.Setup(m => m.Invoke(scope.Object, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => args[0]);
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("2".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAnAssignmentNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Assignment("answer", Node.Literal("42".ToValue()));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        scope.Verify(s => s.SetVariable("answer", "42".ToValue()), Times.Once);
        result.Should().Be("42".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateADefineVariableNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.DefineVariable("answer", Node.Literal("42".ToValue()));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        scope.Verify(s => s.DefineVariable("answer", "42".ToValue()), Times.Once);
        result.Should().Be("42".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAExpressionNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Expression(Node.Literal(8));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be(8.0.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateABinOpNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.BinOp("add", Node.Literal(1), Node.Literal(2));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be(3.0.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAUniOpNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.UniOp("not", Node.Literal(false.ToValue()));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be(true.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAnIfNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.If(Node.Literal(false), Node.Literal("yes"), Node.Literal("no"));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("no".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAWhileNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.While(Node.Literal(false), Node.Literal("I never run!"));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheEvaluatorCanEvaluateAScopeNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Scope(Node.Literal("boo"));

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("boo".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateARaiseNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Raise(Node.Literal("/error/test"), Node.Literal("Test message."), Node.Literal("testvalue"));

        evaluator.Invoking(e => e.Evaluate(new Mock<IScope>().Object, node, evaluator)).Should()
            .Throw<Error>().WithMessage("Test message.").Where(e => e.Value.Equals("testvalue".ToValue()));
    }

    [Test]
    public void TheEvaluatorCanEvaluateATryNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Try(Node.Literal("hello"), null);

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("hello".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateADefineCommandNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.DefineCommand(Node.Literal("test"), Node.Literal("body"), new ListValue(), new ListValue());

        evaluator.Evaluate(scope.Object, node, evaluator);

        scope.Verify(m => m.DefineCommand("test", It.IsAny<ICommand>()));
    }
}