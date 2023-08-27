namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ForDictEvaluatorTests : EvaluatorTestsBase
{
    List<Value> _recordedValues = null!;
    TestCommand _testCommand = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheDictIsEmptyTheBodyIsNotEvaluatedAndAnEmptyValueIsReturned()
    {
        var node = Node.ForDict(Node.Literal("c"), Node.Literal(new DictValue()), CreateLoopBody("c"));

        var result = Evaluate(node);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void WhenTheDictIsNotEmptyTheResultIsTheResultOfTheLastEvaluationOfBody()
    {
        var node = Node.ForDict(Node.Literal("c"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue())), CreateLoopBody("c"));

        var result = Evaluate(node);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void AVariableWithTheNameOfTheIteratorIsDefinedInTheScopePassedToTheBody()
    {
        var node = Node.ForDict(Node.Literal("key"), Node.Literal(new DictValue(1.ToValue())), CreateLoopBody("key"));

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.Invoking(s => s.GetVariable("key")).Should().NotThrow();
    }

    [Test]
    public void TheBodyIsEvaluatedOneTimeForEachKeyInTheDict()
    {
        var node = Node.ForDict(Node.Literal("name"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("name"));

        Evaluate(node);

        _testCommand.Invocations.Should().Be(2);
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAnIteratorVariableDefinedAndAssignedTheValueOfTheCurrentKey()
    {
        var node = Node.ForDict(Node.Literal("b"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("b"));

        Evaluate(node);

        _recordedValues.Should().Equal("a".ToValue(), "b".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAValueVariableDefinedAndAssignedTheValueOfTheCurrentItem()
    {
        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("v"));

        Evaluate(node);

        _recordedValues.Should().Equal(1.ToValue(), 2.ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithTheOuterScopeSetToTheEvaluatorsScope()
    {
        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("k"));

        Evaluate(node);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(Environment.GlobalScope);
    }

    [Test]
    public void IfTheValueAndKeyIteratorsHaveTheSameNameAnErrorIsThrown()
    {
        var node = Node.ForDict(Node.Literal("SAME"), Node.Literal("same"), Node.Literal(new DictValue()), Node.Script());

        this.Invoking(e => e.Evaluate(node)).Should()
            .Throw<ArgError>("key and value iterators can not have the same value 'SAME'.");
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

        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), body);

        var result = Evaluate(node);

        _testCommand.Invocations.Should().Be(1);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void EvaluationOfTheLoopBodyEndsAfterAContinueIsThrownTheAnTheNextIterationIsEvaluated()
    {
        var body = Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable("k"))),
            Node.Raise(
                Node.Literal("/continue/"),
                Node.Literal(Value.Empty),
                Node.Literal(Value.Empty)
            ),
            Node.Command(Node.Literal("test"), new ListValue())
        );

        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), body);

        Evaluate(node);

        _testCommand.Invocations.Should().Be(0);
        _recordedValues.Should().Equal("a".ToValue(), "b".ToValue());
    }

    [SetUp]
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
        return new ForDictEvaluator();
    }
}