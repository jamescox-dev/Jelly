namespace Jelly.Evaluator.Tests;

public class ScriptEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void EachCommandInAScriptIsEvaluatedInOrder()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var output = new List<string>();
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(Environment, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => { output.Add(args[0].ToString()); return args[0]; });
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        Evaluate(node);

        output.Should().BeEquivalentTo(new[] { "1", "2" });
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsTheResultOfTheLastCommandEvaluated()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(Environment, It.IsAny<ListValue>())).Returns<IScope, ListValue>((scope, args) => args[0]);
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        scope.Setup(m => m.GetCommand("command1")).Returns(command.Object);
        scope.Setup(m => m.GetCommand("command2")).Returns(command.Object);
        var node = Node.Script(command1, command2);

        var result = Evaluate(node);

        result.Should().Be("2".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsAnEmptyValueIfItHasNoCommandsToRun()
    {
        var scriptEvaluator = new ScriptEvaluator();
        var evaluator = new Evaluator();
        var node = Node.Script();

        var result = Evaluate(node);

        result.Should().Be(Value.Empty);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ScriptEvaluator();
    }
}