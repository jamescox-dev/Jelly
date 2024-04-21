namespace Jelly.Tests.Evaluator;

public class ScriptEvaluatorTests : EvaluatorTestsBase
{
    List<string> _output = null!;

    [Test]
    public void EachCommandInAScriptIsEvaluatedInOrder()
    {
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        var node = Node.Script(command1, command2);

        Evaluate(node);

        _output.Should().BeEquivalentTo(new[] { "1", "2" });
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsTheResultOfTheLastCommandEvaluated()
    {
        var command1 = Node.Command(Node.Literal("command1".ToValue()), new ListValue(Node.Literal("1".ToValue())));
        var command2 = Node.Command(Node.Literal("command2".ToValue()), new ListValue(Node.Literal("2".ToValue())));
        var node = Node.Script(command1, command2);

        var result = Evaluate(node);

        result.Should().Be("2".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingAScriptIsAnEmptyValueIfItHasNoCommandsToRun()
    {
        var node = Node.Script();

        var result = Evaluate(node);

        result.Should().Be(Value.Empty);
    }

    public override void Setup()
    {
        base.Setup();

        _output = new();

        var command = new Mock<ICommand>();
        command.Setup(m => m.Invoke(Environment, It.IsAny<ListValue>())).Returns<IEnv, ListValue>((env, args) =>
        {
            var value = env.Evaluate(args[0].ToNode());
            _output.Add(value.ToString());
            return value;
        });

        Environment.GlobalScope.DefineCommand("command1", command.Object);
        Environment.GlobalScope.DefineCommand("command2", command.Object);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ScriptEvaluator();
    }
}