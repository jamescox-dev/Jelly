namespace Jelly.Evaluator;

using Jelly.Commands;
using Jelly.Errors;
using Jelly.Parser;
using Jelly.Values;

[TestFixture]
public class EvaluatorTests
{
    [Test]
    public void TheEvaluatorCanEvaluateALiteralNode()
    {
        var evaluator = new Evaluator();
        var node = Node.Literal("Hi".ToValue());

        var result = evaluator.Evaluate(new Mock<IScope>().Object, node);
        
        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAVariableNode()
    {
        var evaluator = new Evaluator();
        Mock<IScope> scope = new Mock<IScope>();
        scope.Setup(m => m.GetVariable("Name")).Returns("Bill".ToValue());
        var node = Node.Variable("Name");
        
        var result = evaluator.Evaluate(scope.Object, node);
        
        result.Should().Be("Bill".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateACommandNode()
    {
        var evaluator = new Evaluator();
        Mock<IScope> scope = new Mock<IScope>();
        Mock<ICommand> command = new Mock<ICommand>();
        scope.Setup(m => m.GetCommand("Foo")).Returns(command.Object);
        var node = Node.Command(Node.Literal("Foo".ToValue()), new ListValue());
        
        var result = evaluator.Evaluate(scope.Object, node);
        
        command.Verify(m => m.Invoke(scope.Object, new ListValue()), Times.Once);
    }

    [Test]
    public void TheEvaluatorCanEvaluateAScriptNode()
    {
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(scope.Object, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => args[0]);
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        var result = evaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("2".ToValue());
    }
}