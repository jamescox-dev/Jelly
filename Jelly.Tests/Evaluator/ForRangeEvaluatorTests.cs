namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class ForRangeEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Scope _scope = null!;
    List<double> _recoredIterators = null!;
    TestCommand _testCommand = null!;
    DictionaryValue _testBody = null!;
    IEvaluator _rootEvaluator = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheStartAndEndValuesAreTheSameResultsIsTheResultOfEvaluatingTheBody()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(0), Node.Literal(1), _testBody);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedForEachValueBetweenTheGivenRangeTheResultIsThatOfTheLastEvaluationOfTheBody()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(1), Node.Literal(10), Node.Literal(1), _testBody);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(10);
        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void EachIterationOfTheBodyIsEvaluatedInAScopeContainingAVariableSetToTheCurrentValueOfTheIterationWithTheOuterScopeSetCorrectyly()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(8), Node.Literal(10), Node.Literal(1), _testBody);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(_scope);
        _recoredIterators.Should().Equal(8.0, 9.0, 10.0);
    }

    [Test]
    public void TheIteratorCanBeIncrementedByASetAmount()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(2), _testBody);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredIterators.Should().Equal(0.0, 2.0, 4.0, 6.0, 8.0, 10.0);
    }

    [Test]
    public void IfTheStepIsZeroAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(0), _testBody);

        _evaluator.Invoking(e =>e.Evaluate(_scope, node, _rootEvaluator)).Should()
            .Throw<ArgError>().WithMessage("step can not be zero.");
    }

    [Test]
    public void TheIteratorCanBeSetToANegativeValue()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(5), Node.Literal(1), Node.Literal(-1), _testBody);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredIterators.Should().Equal(5.0, 4.0, 3.0, 2.0, 1.0);
    }

    [Test]
    public void IfTheStepIsNegativeAndTheEndIsGreaterThanTheStartAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(0), Node.Literal(10), Node.Literal(-2), _testBody);

        _evaluator.Invoking(e =>e.Evaluate(_scope, node, _rootEvaluator)).Should()
            .Throw<ArgError>().WithMessage("step must be positive when start is less than end.");
    }

    [Test]
    public void IfTheStepIsPositiveAndTheEndIsLessThanTheStartAnErrorIsRaised()
    {
        var node = Node.ForRange(Node.Literal("a"), Node.Literal(10), Node.Literal(0), Node.Literal(2), _testBody);

        _evaluator.Invoking(e =>e.Evaluate(_scope, node, _rootEvaluator)).Should()
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

        var node = Node.ForRange(Node.Literal("a"), Node.Literal(1), Node.Literal(10), Node.Literal(1), body);

        _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(0);
        _recoredIterators.Should().Equal(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [SetUp]
    public void Setup()
    {
        _scope = new Scope();
        _recoredIterators = new();
        _testCommand = new TestCommand();
        _testCommand.ReturnValue = "Result!".ToValue();
        _recordCommand = new SimpleCommand((scope, args) => { _recoredIterators.Add(args[0].ToDouble()); return Value.Empty; });
        _scope.DefineCommand("test", _testCommand);
        _scope.DefineCommand("record", _recordCommand);
        _testBody = Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable("a"))),
            Node.Command(Node.Literal("test"), new ListValue()));
        _rootEvaluator = new Evaluator();

        _evaluator = new ForRangeEvaluator();
    }
}