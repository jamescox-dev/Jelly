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
        var node = new NodeBuilder().Literal("Hi".ToValue());

        var result = evaluator.Evaluate(new Mock<IScope>().Object, node);
        
        result.Should().Be("Hi".ToValue());
    }

    [Test]
    public void TheEvaluatorCanEvaluateAVariableNode()
    {
        var evaluator = new Evaluator();
        Mock<IScope> scope = new Mock<IScope>();
        scope.Setup(m => m.GetVariable("Name")).Returns("Bill".ToValue());
        var node = new NodeBuilder().Variable("Name");
        
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
        var builder = new NodeBuilder();
        var node = builder.Command(builder.Literal("Foo".ToValue()), new ListValue());
        
        var result = evaluator.Evaluate(scope.Object, node);
        
        command.Verify(m => m.Invoke(scope.Object, new ListValue()), Times.Once);
    }
}