namespace Jelly.Evaluator.Tests;

using Jelly.Runtime;

[TestFixture]
public class VariableEvaluatorTests
{
    [Test]
    public void EvaluatingAVariableNodeReturnsTheValueOfTheNamedVariableFromTheGivenScope()
    {
        var env = new Env();
        var evaluator = new VariableEvaluator();
        env.GlobalScope.DefineVariable("answer", "42".ToValue());
        var variable = Node.Variable("answer");

        var result = evaluator.Evaluate(env, variable);

        result.Should().Be(new StringValue("42"));
    }
}