namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Values;

public class ScriptEvaluatorTests
{
    [Test]
    public void EachCommandInAScriptIsEvaluatedInOrder()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var output = new List<string>();
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(scope.Object, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => { output.Add(args[0].ToString()); return args[0]; });
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        scriptEvaluator.Evaluate(scope.Object, node, evaluator);

        output.Should().BeEquivalentTo(new[] { "1", "2" });
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsTheResultOfTheLastCommandEvaluated()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(scope.Object, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => args[0]);
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        var result = scriptEvaluator.Evaluate(scope.Object, node, evaluator);

        result.Should().Be("2".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsAnEmptyValueIfItHasNoCommandsToRun()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var node = Node.Script();

        var result = scriptEvaluator.Evaluate(new Mock<IScope>().Object, node, evaluator);

        result.Should().Be(Value.Empty);
    }
}