namespace Jelly.Interpreter.Tests;

[TestFixture]
public class VariableInterpreterTests
{
    [Test]
    public void EvaluatingAVariableNodeReturnsTheValueOfTheNamedVariableFromTheGivenScope()
    {
        var scope = new Scope();
        var interpreter = new VariableInterpreter();
        scope.DefineVariable("answer", new StringValue("42"));
        var variable = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("name"), new StringValue("answer"))
        });

        var result = interpreter.Evaluate(scope, variable, interpreter);

        result.Should().Be(new StringValue("42"));
    }
}