namespace Jelly.Evaluator.Test;

using Jelly.Ast;
using Jelly.Values;

[TestFixture]
public class IfEvaluatorTest
{
    IfEvaluator _evaluator = null!;
    
    Scope _scope = null!;
    Evaluator _rootEvaluator = null!;

    TestCommand _test1Command = null!;
    TestCommand _test2Command = null!;

    [Test]
    public void TheThenBodyOfAnIfNodeIsEvaluatedIfTheConditionEvaluatesToTrueAndTheResultOfTheThenBodyIsReturned()
    {
        var ifNode = Node.If(Node.Literal(true), Node.Command(Node.Literal("test1"), new ListValue()));

        var result = _evaluator.Evaluate(_scope, ifNode, _rootEvaluator);

        _test1Command.ScopePassedToInvoke.Should().Be(_scope);
        result.Should().Be("test1.result".ToValue());
    }

    [Test]
    public void TheThenBodyOfAnIfNodeIsNotEvaluatedIfTheConditionEvaluatesToFalseAndTheResultIsAnEmptyValueIfThereIsNoElseBody()
    {
        var ifNode = Node.If(Node.Literal(false), Node.Command(Node.Literal("test1"), new ListValue()));

        var result = _evaluator.Evaluate(_scope, ifNode, _rootEvaluator);

        _test1Command.ScopePassedToInvoke.Should().Be(null);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheElseBodyOfAnIfNodeIsEvaluatedIfTheConditionEvaluatesToFalseAndTheResultOfTheElseBodyIsReturned()
    {
        var ifNode = Node.If(
            Node.Literal(false), 
            Node.Command(Node.Literal("test1"), new ListValue()),
            Node.Command(Node.Literal("test2"), new ListValue()));

        var result = _evaluator.Evaluate(_scope, ifNode, _rootEvaluator);

        _test2Command.ScopePassedToInvoke.Should().Be(_scope);
        result.Should().Be("test2.result".ToValue());
    }

    [SetUp]
    public void Setup()
    {
        _evaluator = new IfEvaluator();

        _scope = new Scope();
        _rootEvaluator = new Evaluator();

        _test1Command = new TestCommand { ReturnValue = "test1.result".ToValue() };
        _test2Command = new TestCommand { ReturnValue = "test2.result".ToValue() };

        _scope.DefineCommand("test1", _test1Command);
        _scope.DefineCommand("test2", _test2Command);
    }
}