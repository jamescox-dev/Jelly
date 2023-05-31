namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class SimpleCommandTests
{
    [Test]
    public void TheDelegateIsCalledWithTheCurrentEnvironmentAndEachOfItsArgumentsEvaluatedAndTheResultIsReturned()
    {
        var command = new SimpleCommand(TestCommand);
        var env = new Environment();
        var args = new ListValue(Node.Literal(1), Node.Literal(2), Node.Literal(3));
        IEnvironment? passedEnv = null;
        ListValue? passedArgs = null;
        Value TestCommand(IEnvironment env, ListValue args)
        {
            passedEnv = env;
            passedArgs = args;
            return 42.ToValue();
        }

        var result = command.Invoke(env, args);

        result.Should().Be(42.ToValue());
        passedEnv.Should().Be(env);
        ((IEnumerable<Value>?)passedArgs).Should().Equal(1.ToValue(), 2.ToValue(), 3.ToValue());
    }
}