namespace Jelly.Evaluator.Tests;

[TestFixture]
public class EvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheEvaluatorCanEvaluateALiteralNode()
    {
        IEvaluator evaluator = new Evaluator();
        var node = Node.Literal("Hi".ToValue());

        var result = Evaluate(node);

        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateACompositeNode()
    {
        IEvaluator evaluator = new Evaluator();
        var node = Node.Composite(Node.Literal("Hi".ToValue()));

        var result = Evaluate(node);

        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAVariableNode()
    {
        Environment.GlobalScope.DefineVariable("Name", "Bill".ToValue());
        var node = Node.Variable("Name");

        var result = Evaluate(node);

        result.Should().Be("Bill".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateACommandNode()
    {
        Mock<ICommand> command = new Mock<ICommand>();
        Environment.GlobalScope.DefineCommand("Foo", command.Object);
        var node = Node.Command(Node.Literal("Foo".ToValue()), new ListValue());

        var result = Evaluate(node);
        command.Verify(m => m.Invoke(Environment, new ListValue()), Times.Once);
    }

    [Test]
    public void TheEvaluatorCanEvaluateAScriptNode()
    {
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(Environment, It.IsAny<ListValue>())).Returns<IEnvironment, ListValue>((_, args) => args[0]);
        Environment.GlobalScope.DefineCommand("command1", command.Object);
        Environment.GlobalScope.DefineCommand("command2", command.Object);
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        var node = Node.Script(command1, command2);

        var result = Evaluate(node);

        result.Should().Be(Node.Literal("2".ToValue()));
    }

    [Test]
    public void TheEvaluatorCanEvaluateAnAssignmentNode()
    {
        Environment.GlobalScope.DefineVariable("answer", 0.ToValue());
        var node = Node.Assignment("answer", Node.Literal("42".ToValue()));

        var result = Evaluate(node);

        Environment.GlobalScope.GetVariable("answer").Should().Be(42.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateADefineVariableNode()
    {
        var node = Node.DefineVariable("answer", Node.Literal("42".ToValue()));

        var result = Evaluate(node);

        Environment.GlobalScope.GetVariable("answer").Should().Be("42".ToValue());
        result.Should().Be("42".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAExpressionNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Expression(Node.Literal(8));

        var result = Evaluate(node);

        result.Should().Be(8.0.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateABinOpNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.BinOp("add", Node.Literal(1), Node.Literal(2));

        var result = Evaluate(node);

        result.Should().Be(3.0.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAUniOpNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.UniOp("not", Node.Literal(false.ToValue()));

        var result = Evaluate(node);

        result.Should().Be(true.ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAnIfNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.If(Node.Literal(false), Node.Literal("yes"), Node.Literal("no"));

        var result = Evaluate(node);

        result.Should().Be("no".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAWhileNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.While(Node.Literal(false), Node.Literal("I never run!"));

        var result = Evaluate(node);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheEvaluatorCanEvaluateAScopeNode()
    {
        var scope = new Mock<IScope>();
        var node = Node.Scope(Node.Literal("boo"));

        var result = Evaluate(node);

        result.Should().Be("boo".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateARaiseNode()
    {
        var scope = new Mock<IScope>();
        var node = Node.Raise(Node.Literal("/error/test"), Node.Literal("Test message."), Node.Literal("testvalue"));

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<Error>().WithMessage("Test message.").Where(e => e.Value.Equals("testvalue".ToValue()));
    }

    [Test]
    public void TheEvaluatorCanEvaluateATryNode()
    {
        IEvaluator evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var node = Node.Try(Node.Literal("hello"), null);

        var result = Evaluate(node);

        result.Should().Be("hello".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateADefineCommandNode()
    {
        var node = Node.DefineCommand(Node.Literal("test"), Node.Literal("body"), new ListValue(), new ListValue());

        Evaluate(node);

        Environment.GlobalScope.GetCommand("test");
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new Evaluator();
    }
}