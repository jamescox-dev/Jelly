namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class SimpleCommandTests
{
    [Test]
    public void TheDelegateIsCalledWithTheItsArgumentsEvaluatedAndTheResultIsReturned()
    {
        var command = new SimpleCommand(TestCommand);
        var env = new Env();
        var args = new ListValue(Node.Literal(1), Node.Literal(2), Node.Literal(3));
        ListValue? passedArgs = null;
        Value TestCommand(ListValue args)
        {
            passedArgs = args;
            return 42.ToValue();
        }

        var result = command.Invoke(env, args);

        result.Should().Be(42.ToValue());
        ((IEnumerable<Value>?)passedArgs).Should().Equal(1.ToValue(), 2.ToValue(), 3.ToValue());
    }
}