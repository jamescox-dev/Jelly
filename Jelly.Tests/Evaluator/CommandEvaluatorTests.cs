namespace Jelly.Evaluator.Tests;

[TestFixture]
public class CommandEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheNamedCommandIsRetrievedFromTheCurrentScopeAndEvaluated()
    {
        Environment.GlobalScope.DefineCommand("greet", new TestCommand { ReturnValue = 42.ToValue() });
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), new ListValue());

        var result = Evaluate(commandNode);

        result.Should().Be(42.ToValue());
    }

    [Test]
    public void ArgumentNodesArePassedUnevaluated()
    {
        var command = new TestCommand() { ReturnValue = Node.Literal(Value.Empty) };
        Environment.GlobalScope.DefineCommand("greet", command);
        var args = new ListValue(Node.Literal("Vic".ToValue()), Node.Literal("Bob".ToValue()));
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), args);

        Evaluate(commandNode);

        ((IComparable<Value>?)command.ArgsPassedToInvoke).Should().Be(args);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new CommandEvaluator();
    }
}