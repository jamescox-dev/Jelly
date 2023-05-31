namespace Jelly.Commands.Tests;

[TestFixture]
public class SimpleCommandTests
{
    [Test]
    public void TheDelegateCalledWithEachOfItsArgumentsEvaluated()
    {
        var command = new SimpleCommand(TestCommand);
        var mockScope = new Mock<IScope>();
        var testArgs = new ListValue("1".ToValue(), "2".ToValue(), "3".ToValue());
        IScope? passedScope = null;
        ListValue? passedArgs = null;
        Value TestCommand(IEnvironment scope, ListValue args)
        {
            passedScope = scope;
            passedArgs = args;
            return "42".ToValue();
        }

        var result = command.Invoke(mockScope.Object, testArgs);

        result.Should().Be("42".ToValue());
        passedScope.Should().Be(mockScope.Object);
        ((IEnumerable<Value>?)passedArgs).Should().BeEquivalentTo(testArgs);
    }
}