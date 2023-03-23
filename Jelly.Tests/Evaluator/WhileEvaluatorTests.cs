namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Values;

[TestFixture]
public class WhileEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _evaluator = new WhileEvaluator();
        _rootEvaluator = new Evaluator();
        _scope = new Scope();
    }

    [Test]
    public void TheBodyIsEvaluatedWhileEvaluatingTheConditionIsNotZeroTheResultIsTheResultOfTheLastCommandEvaluatedInTheBody()
    {
        var condCmd = new CounterCommand { Count = 4, Increment = -1};
        var bodyCmd = new CounterCommand();
        _scope.DefineCommand("cond", condCmd);
        _scope.DefineCommand("body", bodyCmd);
        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())),
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
        
        var result = _evaluator.Evaluate(_scope, whileNode, _rootEvaluator);

        bodyCmd.CallCount.Should().Be(3);
        result.Should().Be("3".ToValue());
    }

    [Test]
    public void TheWhileCommandReturnsAnEmptyValueIfTheBodyNeverRuns()
    {
        var condCmd = new CounterCommand { Count = 1, Increment = -1};
        var bodyCmd = new CounterCommand();
        _scope.DefineCommand("cond", condCmd);
        _scope.DefineCommand("body", bodyCmd);
        var whileNode = Node.While(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())), 
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
        
        var result = _evaluator.Evaluate(_scope, whileNode, _rootEvaluator);

        bodyCmd.CallCount.Should().Be(0);
        result.Should().Be(Value.Empty);
    }
}