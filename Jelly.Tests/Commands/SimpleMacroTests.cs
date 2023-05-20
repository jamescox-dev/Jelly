namespace Jelly.Commands.Tests;

[TestFixture]
public class SimpleMacroTests
{
    [Test]
    public void TheDelegatePassedInTheConstructorIsCalledWithTheArgumentsPassedToInvokeAndItReturnValueReturned()
    {
        var macro = new SimpleMacro(TestMacro);
        var mockScope = new Mock<IScope>();
        var testArgs = new ListValue("1".ToValue(), "2".ToValue(), "3".ToValue());
        IScope? passedScope = null;
        ListValue? passedArgs = null;
        Value TestMacro(IScope scope, ListValue args)
        {
            passedScope = scope;
            passedArgs = args;
            return "42".ToValue();
        }

        var result = macro.Invoke(mockScope.Object, testArgs);

        result.Should().Be("42".ToValue());
        passedScope.Should().Be(mockScope.Object);
        ((IEnumerable<Value>?)passedArgs).Should().BeEquivalentTo(testArgs);
    }

    [Test]
    public void TheMacroIsFlaggedToNotHaveItArgumentEvaluatedButItReturnValueEvaluated()
    {
        var macro = new SimpleMacro((s, a) => Value.Empty);

        macro.EvaluationFlags.Should().Be(EvaluationFlags.RetrunValue);
    }
}