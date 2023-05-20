namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ForListEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Scope _scope = null!;
    List<Value> _recoredValues = null!;
    TestCommand _testCommand = null!;
    IEvaluator _rootEvaluator = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheListIsEmptyTheBodyIsNotEvaluatedAndAnEmptyValueIsReturned()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue()), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void WhenTheListIsNotEmptyTheResultIsTheResultOfTheLastEvaluationOfBody()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue(1.ToValue())), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void AVairableWithTheNameOfTheIteratorIsDefinedInTheScopePassedToTheBody()
    {
        var node = Node.ForList(Node.Literal("c"), Node.Literal(new ListValue(1.ToValue())), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.ScopePassedToInvoke?.Invoking(s => s.GetVariable("c")).Should().NotThrow();
    }

    [Test]
    public void TheBodyIsEvaluatedOneTimeForEachItemInTheList()
    {
        var node = Node.ForList(Node.Literal("b"), Node.Literal(new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue())), CreateLoopBody("b"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(3);
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIteratorVariableDefinedAndAssignedTheValueOfTheCurrentItem()
    {
        var node = Node.ForList(Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())), CreateLoopBody("a"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredValues.Should().Equal("a".ToValue(), "b".ToValue(), "c".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIndexVariableDefinedAndAssignedTheIndexOfTheCurrentItem()
    {
        var node = Node.ForList(Node.Literal("i"), Node.Literal("a"), Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())), CreateLoopBody("i"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredValues.Should().Equal(1.ToValue(), 2.ToValue(), 3.ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithTheOuterScopeSetToTheEvaluatorsScope()
    {
        var node = Node.ForList(Node.Literal("i"), Node.Literal("a"), Node.Literal(new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue())), CreateLoopBody("a"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(_scope);
    }

    [Test]
    public void IfTheValueAndIndexIteratorsHaveTheSameNameAnErrorIsThrown()
    {
        var node = Node.ForList(Node.Literal("SAME"), Node.Literal("same"), Node.Literal(new ListValue()), Node.Script());

        _evaluator.Invoking(e => e.Evaluate(_scope, node, _rootEvaluator)).Should()
            .Throw<ArgError>("index and value interators can not have the same value 'SAME'.");
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

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(1);
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

        _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(0);
        _recoredValues.Should().Equal("a".ToValue(), "b".ToValue(), "c".ToValue());
    }

    [SetUp]
    public void Setup()
    {
        _scope = new Scope();
        _recoredValues = new();
        _testCommand = new TestCommand();
        _testCommand.ReturnValue = "Result!".ToValue();
        _recordCommand = new SimpleCommand((scope, args) => {
            if (args.Count == 1)
            {
                _recoredValues.Add(args[0]);
            }
            return Value.Empty;
        });
        _scope.DefineCommand("test", _testCommand);
        _scope.DefineCommand("record", _recordCommand);
        _rootEvaluator = new Evaluator();

        _evaluator = new ForListEvaluator();
    }

    static DictionaryValue CreateLoopBody(string iteratorName)
    {
        return Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable(iteratorName))),
            Node.Command(Node.Literal("test"), new ListValue()));
    }
}