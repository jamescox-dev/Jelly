namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Values;

[TestFixture]
public class ScopeEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Scope _scope = null!;
    Evaluator _rootEvaluator = null!;

    [Test]
    public void EvaluatingAScopeNodeReturnsTheResultOfEvaluatingItsBody()
    {
        var scopeNode = Node.Scope(Node.Literal("hi"));

        var result = _evaluator.Evaluate(_scope, scopeNode, _rootEvaluator);

        result.Should().Be("hi".ToValue());
    }

    [Test]
    public void EvaluatingAScopeNodeEvauatesItsBodyWithANewScopeWithTheCurrentScopeSetAsItsOuterScope()
    {
        IScope? passedScope = null;
        var testCommand = new SimpleCommand((scope, _) => { passedScope = scope; return Value.Empty; });
        _scope.DefineCommand("test", testCommand);
        var scopeNode = Node.Scope(Node.Command(Node.Literal("test"), new ListValue()));

        var result = _evaluator.Evaluate(_scope, scopeNode, _rootEvaluator);

        passedScope.Should().NotBeNull();
        passedScope!.OuterScope.Should().Be(_scope);
    }

    [SetUp]
    public void Setup()
    {
        _scope = new();
        _rootEvaluator = new();
        _evaluator = new ScopeEvaluator();
    }
}