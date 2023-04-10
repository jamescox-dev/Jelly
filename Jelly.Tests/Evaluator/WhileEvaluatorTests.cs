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
    public void TheWhileNodeEvaluatesTpAnEmptyValueIfTheBodyNeverRuns()
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

    [Test]
    public void TheLoopEndsAfterABreakIsThrownTheResultIsAnEmptyValue()
    {
        var condCmd = new CounterCommand { Count = 4, Increment = -1};
        var bodyCmd = new CounterCommand();
        _scope.DefineCommand("cond", condCmd);
        _scope.DefineCommand("body", bodyCmd);
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
        
        var result = _evaluator.Evaluate(_scope, whileNode, _rootEvaluator);

        bodyCmd.CallCount.Should().Be(0);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheExecutionOfTheLoopBodyEndsAfterAContinueIsThrownAndTheConditionIsReavaluatedAndTheLoopContinues()
    {
        var condCmd = new CounterCommand { Count = 4, Increment = -1};
        var bodyCmd = new CounterCommand();
        _scope.DefineVariable("count", 0.ToValue());
        _scope.DefineCommand("cond", condCmd);
        _scope.DefineCommand("body", bodyCmd);
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
        
        var result = _evaluator.Evaluate(_scope, whileNode, _rootEvaluator);

        bodyCmd.CallCount.Should().Be(1);
        result.Should().Be(1.ToValue());
    }
}