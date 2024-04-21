namespace Jelly.Tests.Evaluator;

[TestFixture]
public class WhileEvaluatorTests : EvaluatorTestsBase
{
    CounterCommand _condCmd = null!;
    CounterCommand _bodyCmd = null!;

    public override void Setup()
    {
        base.Setup();

        _condCmd = new CounterCommand { Increment = -1 };
        _bodyCmd = new CounterCommand();

        Environment.GlobalScope.DefineCommand("cond", _condCmd);
        Environment.GlobalScope.DefineCommand("body", _bodyCmd);
    }

    [Test]
    public void TheBodyIsEvaluatedWhileEvaluatingTheConditionIsNotZeroTheResultIsTheResultOfTheLastCommandEvaluatedInTheBody()
    {
        _condCmd.Count = 4;

        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())),
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));

        var result = Evaluate(whileNode);

        _bodyCmd.CallCount.Should().Be(3);
        result.Should().Be("3".ToValue());
    }

    [Test]
    public void TheWhileNodeEvaluatesToAnEmptyValueIfTheBodyNeverRuns()
    {
        _condCmd.Count = 1;

        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())),
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));

        var result = Evaluate(whileNode);

        _bodyCmd.CallCount.Should().Be(0);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheLoopEndsAfterABreakIsThrownTheResultIsAnEmptyValue()
    {
        _condCmd.Count = 4;

        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())),
            Node.Script(
                Node.Raise(
                    Node.Literal("/break/"),
                    Node.Literal(Value.Empty),
                    Node.Literal(Value.Empty)
                ),
                Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue()))
            )
        );

        var result = Evaluate(whileNode);

        _bodyCmd.CallCount.Should().Be(0);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheExecutionOfTheLoopBodyEndsAfterAContinueIsThrownAndTheConditionIsReevaluatedAndTheLoopContinues()
    {
        _condCmd.Count = 4;
        Environment.GlobalScope.DefineVariable("count", 0.ToValue());

        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())),
            Node.Script(
                Node.Assignment("count", Node.BinOp("add", Node.Variable("count"), Node.Literal(1))),
                Node.If(
                    Node.BinOp("lte", Node.Variable("count"), Node.Literal(2)),
                    Node.Raise(
                        Node.Literal("/continue/"),
                        Node.Literal(Value.Empty),
                        Node.Literal(Value.Empty)
                    )
                ),
                Node.Command(Node.Literal("body".ToValue()), new ListValue())
            )
        );

        var result = Evaluate(whileNode);

        _bodyCmd.CallCount.Should().Be(1);
        result.Should().Be(1.ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new WhileEvaluator();
    }
}