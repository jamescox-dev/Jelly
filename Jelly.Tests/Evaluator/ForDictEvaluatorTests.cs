namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class ForDictEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Scope _scope = null!;
    List<Value> _recoredValues = null!;
    TestCommand _testCommand = null!;
    IEvaluator _rootEvaluator = null!;
    SimpleCommand _recordCommand = null!;

    [Test]
    public void WhenTheDictIsEmptyTheBodyIsNotEvaluatedAndAnEmptyValueIsReturned()
    {
        var node = Node.ForDict(Node.Literal("c"), Node.Literal(new DictionaryValue()), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        result.Should().Be(Value.Empty);
    }

    [Test]
    public void WhenTheDictIsNotEmptyTheResultIsTheResultOfTheLastEvaluationOfBody()
    {
        var node = Node.ForDict(Node.Literal("c"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue())), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        result.Should().Be("Result!".ToValue());
    }

    [Test]
    public void AVairableWithTheNameOfTheIteratorIsDefinedInTheScopePassedToTheBody()
    {
        var node = Node.ForDict(Node.Literal("c"), Node.Literal(new DictionaryValue(1.ToValue())), CreateLoopBody("c"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.ScopePassedToInvoke?.Invoking(s => s.GetVariable("c")).Should().NotThrow();
    }

    [Test]
    public void TheBodyIsEvaluatedOneTimeForEachItemInTheDict()
    {
        var node = Node.ForDict(Node.Literal("b"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("b"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(2);
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIteratorVariableDefinedAndAssignedTheValueOfTheCurrentItem()
    {
        var node = Node.ForDict(Node.Literal("b"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("b"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredValues.Should().Equal(1.ToValue(), 2.ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithAIndexVariableDefinedAndAssignedTheKeyOfTheCurrentItem()
    {
        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("k"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _recoredValues.Should().Equal("a".ToValue(), "b".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInAScopeWithTheOuterScopeSetToTheEvaluatorsScope()
    {
        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), CreateLoopBody("k"));

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.ScopePassedToInvoke?.OuterScope.Should().Be(_scope);
    }

    [Test]
    public void IfTheValueAndKeyIteratorsHaveTheSameNameAnErrorIsThrown()
    {
        var node = Node.ForDict(Node.Literal("SAME"), Node.Literal("same"), Node.Literal(new DictionaryValue()), Node.Script());

        _evaluator.Invoking(e => e.Evaluate(_scope, node, _rootEvaluator)).Should()
            .Throw<ArgError>("key and value interators can not have the same value 'SAME'.");
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

        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), body);

        var result = _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(1);
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

        var node = Node.ForDict(Node.Literal("k"), Node.Literal("v"), Node.Literal(new DictionaryValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())), body);

        _evaluator.Evaluate(_scope, node, _rootEvaluator);

        _testCommand.Invokations.Should().Be(0);
        _recoredValues.Should().Equal("a".ToValue(), "b".ToValue());
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

        _evaluator = new ForDictEvaluator();
    }

    static DictionaryValue CreateLoopBody(string iteratorName)
    {
        return Node.Script(
            Node.Command(Node.Literal("record"), new ListValue(Node.Variable(iteratorName))),
            Node.Command(Node.Literal("test"), new ListValue()));
    }
}