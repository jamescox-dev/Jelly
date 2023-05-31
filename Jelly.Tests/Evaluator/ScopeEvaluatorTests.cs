namespace Jelly.Evaluator.Tests;

[TestFixture]
public class ScopeEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void EvaluatingAScopeNodeReturnsTheResultOfEvaluatingItsBody()
    {
        var scopeNode = Node.Scope(Node.Literal("hi"));

        var result = Evaluate(scopeNode);

        result.Should().Be("hi".ToValue());
    }

    [Test]
    public void EvaluatingAScopeNodeEvaluatesItsBodyWithANewScopeWithTheCurrentScopeSetAsItsOuterScope()
    {
        IScope? passedScope = null;
        var testCommand = new SimpleCommand((env, _) => { passedScope = env.CurrentScope; return Value.Empty; });
        Environment.GlobalScope.DefineCommand("test", testCommand);
        var scopeNode = Node.Scope(Node.Command(Node.Literal("test"), new ListValue()));

        var result = Evaluate(scopeNode);

        passedScope.Should().NotBeNull();
        passedScope!.OuterScope.Should().Be(Environment.GlobalScope);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new ScopeEvaluator();
    }
}