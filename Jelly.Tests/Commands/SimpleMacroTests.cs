namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class SimpleMacroTests
{
    [Test]
    public void TheDelegateIsCalledWithTheCurrentEnvironmentAndEachOfItsArgumentsUnevaluatedAndTheResultEvaluatedAndReturned()
    {
        var macro = new SimpleMacro(TestMacro);
        var env = new Environment();
        var args = new ListValue(Node.Literal(1), Node.Literal(2), Node.Literal(3));
        IEnvironment? passedEnv = null;
        List<Value>? passedArgs = null;
        Value TestMacro(IEnvironment env, ListValue args)
        {
            passedEnv = env;
            passedArgs = args.ToList();
            return Node.Literal(42);
        }

        var result = macro.Invoke(env, args);

        result.Should().Be(42.ToValue());
        passedEnv.Should().Be(env);
        passedArgs.Should().Equal(args.ToList());
    }
}