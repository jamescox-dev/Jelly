namespace Jelly.Tests.Evaluator;

[TestFixture]
public class ForListEvaluatorTests : EvaluatorTestsBase
{
    List<Value> _recordedValues = null!;
    TestCommand _testCommand = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheListIsEmptyTheBodyIsNotEvaluatedAndAnEmptyValueIsReturned()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue()), CreateLoopBody("c"));

        var result = Evaluate(node);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void WhenTheListIsNotEmptyTheResultIsTheResultOfTheLastEvaluationOfBody()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue(1.ToValue())), CreateLoopBody("c"));

        var result = Evaluate(node);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void AVariableWithTheNameOfTheIteratorIsDefinedInTheScopePassedToTheBody()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue(1.ToValue())), CreateLoopBody("c"));

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.Invoking(s => s.GetVariable("c")).Should().NotThrow();
    }

    [Test]
    public void TheBodyIsEvaluatedOneTimeForEachItemInTheList()
    {
        var node = Node.ForList(Node.Literal("b"), Node.Literal(new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue())), CreateLoopBody("b"));

        Evaluate(node);

        _testCommand.Invocations.Should().Be(3);
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIteratorVariableDefinedAndAssignedTheValueOfTheCurrentItem()
    {
        var node = Node.ForList(Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())), CreateLoopBody("a"));

        Evaluate(node);

        _recordedValues.Should().Equal("a".ToValue(), "b".ToValue(), "c".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIndexVariableDefinedAndAssignedTheIndexOfTheCurrentItem()
    {
        var node = Node.ForList(Node.Literal("i"), Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())), CreateLoopBody("i"));

        Evaluate(node);

        _recordedValues.Should().Equal(1.ToValue(), 2.ToValue(), 3.ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithTheOuterScopeSetToTheEvaluatorsScope()
    {
        var node = Node.ForList(Node.Literal("i"), Node.Literal("a"), Node.Literal(new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue())), CreateLoopBody("a"));

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(Environment.GlobalScope);
    }

    [Test]
    public void IfTheValueAndIndexIteratorsHaveTheSameNameAnErrorIsThrown()
    {
        var node = Node.ForList(Node.Literal("SAME"), Node.Literal("same"), Node.Literal(new ListValue()), Node.Script());

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<ArgError>("index and value iterators can not have the same value 'SAME'.");
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

        var node = Node.ForList(Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue())), body);

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

        var node = Node.ForList(Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())), body);

        Evaluate(node);

        _testCommand.Invocations.Should().Be(0);
        _recordedValues.Should().Equal("a".ToValue(), "b".ToValue(), "c".ToValue());
    }

    public override void Setup()
    {
        base.Setup();

        _recordedValues = new();
        _testCommand = new TestCommand
        {
            ReturnValue = "Result!".ToValue()
        };
        _recordCommand = new SimpleCommand((args) =>
        {
            if (args.Count == 1)
            {
                _recordedValues.Add(args[0]);
            }
            return Value.Empty;
        });
        Environment.GlobalScope.DefineCommand("test", _testCommand);
        Environment.GlobalScope.DefineCommand("record", _recordCommand);
    }

    static DictValue CreateLoopBody(string iteratorName)
    {
        return Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable(iteratorName))),
            Node.Command(Node.Literal("test"), new ListValue()));
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ForListEvaluator();
    }
}