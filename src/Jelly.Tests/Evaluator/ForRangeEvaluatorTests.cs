namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ForRangeEvaluatorTests : EvaluatorTestsBase
{
    List<double> _recordedIterators = null!;
    TestCommand _testCommand = null!;
    DictValue _testBody = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheStartAndEndValuesAreTheSameTheResultIsTheResultOfEvaluatingTheBody()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(0), Node.Literal(1), _testBody);

        var result = Evaluate(node);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void WhenTheStartAndEndValuesAreTheSameAndNoStepIsGivenTheResultIsTheResultOfEvaluatingTheBody()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(0), _testBody);

        var result = Evaluate(node);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedForEachValueBetweenTheGivenRangeTheResultIsThatOfTheLastEvaluationOfTheBody()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(1), Node.Literal(10), Node.Literal(1), _testBody);

        var result = Evaluate(node);

        _testCommand.Invocations.Should().Be(10);
        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void EachIterationOfTheBodyIsEvaluatedInAScopeContainingAVariableSetToTheCurrentValueOfTheIterationWithTheOuterScopeSetCorrectly()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(8), Node.Literal(10), Node.Literal(1), _testBody);

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(Environment.GlobalScope);
        _recordedIterators.Should().Equal(8.0, 9.0, 10.0);
    }

    [Test]
    public void WhenAStepIsNotGivenAndStartIsLessThanEndADefaultStepOfOneIsGiven()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(8), Node.Literal(10), _testBody);

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(Environment.GlobalScope);
        _recordedIterators.Should().Equal(8.0, 9.0, 10.0);
    }

    [Test]
    public void WhenAStepIsNotGivenAndStartIsGreaterThanEndADefaultStepOfNegativeOneIsGiven()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(10), Node.Literal(8), _testBody);

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(Environment.GlobalScope);
        _recordedIterators.Should().Equal(10.0, 9.0, 8.0);
    }

    [Test]
    public void TheIteratorCanBeIncrementedByASetAmount()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(2), _testBody);

        Evaluate(node);

        _recordedIterators.Should().Equal(0.0, 2.0, 4.0, 6.0, 8.0, 10.0);
    }

    [Test]
    public void IfTheStepIsZeroAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(0), _testBody);

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<ArgError>().WithMessage("step can not be zero.");
    }

    [Test]
    public void TheIteratorCanBeSetToANegativeValue()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(5), Node.Literal(1), Node.Literal(-1), _testBody);

        Evaluate(node);

        _recordedIterators.Should().Equal(5.0, 4.0, 3.0, 2.0, 1.0);
    }

    [Test]
    public void IfTheStepIsNegativeAndTheEndIsGreaterThanTheStartAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(-2), _testBody);

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<ArgError>().WithMessage("step must be positive when start is less than end.");
    }

    [Test]
    public void IfTheStepIsPositiveAndTheEndIsLessThanTheStartAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(10), Node.Literal(0), Node.Literal(2), _testBody);

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<ArgError>().WithMessage("step must be negative when start is greater than end.");
    }

    [Test]
    public void EvaluationOfTheLoopBodyEndsAfterABreakIsThrownTheResultIsAnEmptyValue()
    {
        var body = Node.Script(
            Node.Command(Node.Literal("test"), new ListValue()),
            Node.Raise(
                Node.Literal("/break/"),
                Node.Literal(Value.Empty),
                Node.Literal(Value.Empty)
            ),
            Node.Command(Node.Literal("test"), new ListValue())
        );

        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(2), body);

        var result = Evaluate(node);

        _testCommand.Invocations.Should().Be(1);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void EvaluationOfTheLoopBodyEndsAfterAContinueIsThrownTheAnTheNextIterationIsEvaluated()
    {
        var body = Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable("a"))),
            Node.Raise(
                Node.Literal("/continue/"),
                Node.Literal(Value.Empty),
                Node.Literal(Value.Empty)
            ),
            Node.Command(Node.Literal("test"), new ListValue())
        );

        var node = Node.ForRange(Node.Literal("a"), Node.Literal(1), Node.Literal(10), Node.Literal(1), body);

        Evaluate(node);

        _testCommand.Invocations.Should().Be(0);
        _recordedIterators.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [SetUp]
    public override void Setup()
    {
        base.Setup();

        _recordedIterators = new();
        _testCommand = new TestCommand
        {
            ReturnValue = "Result!".ToValue()
        };
        _recordCommand = new SimpleCommand((args) => { _recordedIterators.Add(args[0].ToDouble()); return Value.Empty; });
        Environment.GlobalScope.DefineCommand("test", _testCommand);
        Environment.GlobalScope.DefineCommand("record", _recordCommand);
        _testBody = Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable("a"))),
            Node.Command(Node.Literal("test"), new ListValue()));
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ForRangeEvaluator();
    }
}