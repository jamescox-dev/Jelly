namespace Jelly.Evaluator.Tests;

using Jelly.Values;

[TestFixture]
public class LiteralEvaluatorTests
{
    [Test]
    public void EvaluatingALiteralNodeReturnsTheNodesValue()
    {
        var scope = new Scope();
        var interpreter = new LiteralEvaluator();
        var literal = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("value"), new StringValue("hello, world"))
        });

        var result = interpreter.Evaluate(scope, literal, interpreter);

        result.Should().Be(new StringValue("hello, world"));
    }
}