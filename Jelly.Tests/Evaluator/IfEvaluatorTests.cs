namespace Jelly.Evaluator.Tests;

[TestFixture]
public class IfEvaluatorTest : EvaluatorTestsBase
{
    TestCommand _test1Command = null!;
    TestCommand _test2Command = null!;

    [Test]
    public void TheThenBodyOfAnIfNodeIsEvaluatedIfTheConditionEvaluatesToTrueAndTheResultOfTheThenBodyIsReturned()
    {
        var ifNode = Node.If(Node.Literal(true), Node.Command(Node.Literal("test1"), new ListValue()));

        var result = Evaluate(ifNode);

        _test1Command.EnvironmentPassedToInvoke.Should().Be(Environment);
        result.Should().Be("test1.result".ToValue());
    }

    [Test]
    public void TheThenBodyOfAnIfNodeIsNotEvaluatedIfTheConditionEvaluatesToFalseAndTheResultIsAnEmptyValueIfThereIsNoElseBody()
    {
        var ifNode = Node.If(Node.Literal(false), Node.Command(Node.Literal("test1"), new ListValue()));

        var result = Evaluate(ifNode);

        _test1Command.EnvironmentPassedToInvoke.Should().Be(null);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheElseBodyOfAnIfNodeIsEvaluatedIfTheConditionEvaluatesToFalseAndTheResultOfTheElseBodyIsReturned()
    {
        var ifNode = Node.If(
            Node.Literal(false),
            Node.Command(Node.Literal("test1"), new ListValue()),
            Node.Command(Node.Literal("test2"), new ListValue()));

        var result = Evaluate(ifNode);

        _test2Command.EnvironmentPassedToInvoke.Should().Be(Environment);
        result.Should().Be("test2.result".ToValue());
    }

    public override void Setup()
    {
        base.Setup();

        _test1Command = new TestCommand { ReturnValue = "test1.result".ToValue() };
        _test2Command = new TestCommand { ReturnValue = "test2.result".ToValue() };

        Environment.GlobalScope.DefineCommand("test1", _test1Command);
        Environment.GlobalScope.DefineCommand("test2", _test2Command);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new IfEvaluator();
    }
}