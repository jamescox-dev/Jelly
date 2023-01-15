namespace Jelly.Interpreter.Tests;

[TestFixture]
public class LiteralInterpreterTests
{
    [Test]
    public void EvaluatingALiteralNodeReturnsTheNodesValue()
    {
        var interpreter = new LiteralInterpreter();
        var literal = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("value"), new StringValue("hello, world"))
        });

        var result = interpreter.Evaluate(literal, interpreter);

        result.Should().Be(new StringValue("hello, world"));
    }
}