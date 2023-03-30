namespace Jelly.Commands.Tests;

using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class WrappedCommandTests
{
    Scope _scope = null!;

    string[] _passedParams = null!;

    Mock<ITypeMarshaller> _mockTypeMarshaller = null!;

    [SetUp]
    public void Setup()
    {
        _scope = new();
        _mockTypeMarshaller = new();
        _passedParams = Array.Empty<string>();
    }

    [Test]
    public void WrappedCommandIsFlaggedForArgumentEvaluation()
    {
        var command = new WrappedCommand(() => {}, _mockTypeMarshaller.Object);

        command.EvaluationFlags.Should().Be(EvaluationFlags.Arguments);
    }

    [Test]
    public void TheWrappedFunctionIsCalled()
    {
        var called = false;
        var func = () => { called = true; };
        var command = new WrappedCommand(func, _mockTypeMarshaller.Object);

        var result = command.Invoke(_scope, new ListValue());

        called.Should().BeTrue();
    }

    [Test]
    public void WhenTheWrappedFunctionsReturnValueIsMarshalledToAJellyValue()
    {
        var returnValue = 42;
        var func = () => returnValue;
        var command = new WrappedCommand(func, _mockTypeMarshaller.Object);

        var result = command.Invoke(_scope, new ListValue());

        _mockTypeMarshaller.Verify(m => m.Marshal(returnValue));
    }

    [Test]
    public void EachArgumentPassedToTheCommandIsMarshalledIntoANativeClrTypeBeforeBegingPassedToTheWrappedDelegate()
    {
        var passedA = 0;
        var passedB = "";
        var passedC = true;
        var func = (int a, string b, bool c) => { passedA = a; passedB = b; passedC = c; };
        var command = new WrappedCommand(func, _mockTypeMarshaller.Object);
        var jellyA = "42".ToValue();
        var jellyB = "jelly".ToValue();
        var jellyC = "0.0".ToValue();
        var args = new ListValue(jellyA, jellyB, jellyC);
        _mockTypeMarshaller.Setup(m => m.Marshal(jellyA, typeof(int))).Returns(42);
        _mockTypeMarshaller.Setup(m => m.Marshal(jellyB, typeof(string))).Returns("jelly");
        _mockTypeMarshaller.Setup(m => m.Marshal(jellyC, typeof(bool))).Returns(false);

        command.Invoke(_scope, args);

        _mockTypeMarshaller.Verify(m => m.Marshal(jellyA, typeof(int)));
        _mockTypeMarshaller.Verify(m => m.Marshal(jellyB, typeof(string)));
        _mockTypeMarshaller.Verify(m => m.Marshal(jellyC, typeof(bool)));
        passedA.Should().Be(42);
        passedB.Should().Be("jelly");
        passedC.Should().BeFalse();
    }

    [Test]
    public void WhenNotEnoughArgumentsArePassedToTheCommandAnErrorIsThrown()
    {
        var func = (int a, int b) => {};
        var command = new WrappedCommand(func, _mockTypeMarshaller.Object);
        var args = new ListValue(8.ToValue());
        _mockTypeMarshaller.Setup(m => m.Marshal(It.IsAny<Value>(), typeof(int))).Returns(0);

        command.Invoking(c => c.Invoke(_scope, args)).Should()
            .Throw<ArgError>().WithMessage("Expected 'b' argument.");
    }

    [Test]
    public void WhenTooManyArgumentsArePassedToTheCommandAnErrorIsThrown()
    {
        var func = (int a) => {};
        var command = new WrappedCommand(func, _mockTypeMarshaller.Object);
        var args = new ListValue(8.ToValue(), 16.ToValue());
        _mockTypeMarshaller.Setup(m => m.Marshal(It.IsAny<Value>(), typeof(int))).Returns(0);

        command.Invoking(c => c.Invoke(_scope, args)).Should()
            .Throw<ArgError>().WithMessage("Unexpected argument '16'.");
    }

    [Test]
    public void WhenTheCommandHasOptionalParametersThatAreMissingTheDefaultValuesArePassed()
    {
        var passedA = 0;
        var passedB = "";
        var passedC = true;
        void Func(int a, string b = "jelly", bool c = false) { passedA = a; passedB = b; passedC = c; };
        var command = new WrappedCommand(Func, _mockTypeMarshaller.Object);
        var args = new ListValue("42".ToValue());
        _mockTypeMarshaller.Setup(m => m.Marshal(It.IsAny<Value>(), typeof(int))).Returns(42);

        command.Invoke(_scope, args);
        passedA.Should().Be(42);
        passedB.Should().Be("jelly");
        passedC.Should().BeFalse();
    }
        
    [Test]
    public void WhenTheWrappedDelegateHasAParamsArgumentAndTheCommandRecivesMoreThanTheNumberOfRequiredAndOptionalArgumentsTheRestOfTheArgumentsArePassedToTheParamsArgument()
    {
        var command = new WrappedCommand(FuncWithParams, _mockTypeMarshaller.Object);
        var args = new ListValue("42".ToValue(), "jelly".ToValue(), "a".ToValue(), "b".ToValue(), "c".ToValue());
        _mockTypeMarshaller.Setup(m => m.Marshal(It.IsAny<Value>(), typeof(string))).Returns<Value, Type>((v, _) => v.ToString());

        command.Invoke(_scope, args);
        _passedParams.SequenceEqual(new[] {"a", "b", "c"}).Should().BeTrue();
    }

    void FuncWithParams(string a, string b = "jelly", params string[] c) 
    {  
        _passedParams = c;
    }
}