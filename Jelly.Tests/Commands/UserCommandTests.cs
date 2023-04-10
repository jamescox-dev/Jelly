namespace Jelly.Commands.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Values;

[TestFixture]
public class UserCommandTests
{
    ICommand _userCommandNoArgs = null!;
    ICommand _userCommand2Args = null!;
    ICommand _userCommand2To4Args = null!;
    ICommand _userCommandVarArgs = null!;

    IEvaluator _rootEvaluator = null!;

    Scope _scope = null!;

    IScope? _passedScope = null;

    DictionaryValue _userCommandBody = null!;

    [Test]
    public void InvokingAUserCommandEvaluatesTheBodyAndReturnsTheResult()
    {
        var result = _userCommandNoArgs.Invoke(_scope, new ListValue());

        result.Should().Be("test-command-result".ToValue());
    }

    [Test]
    public void TheBodyOfTheCommandIsEvaluatedInANewScopeWithinTheCurrentScope()
    {
        _userCommandNoArgs.Invoke(_scope, new ListValue());

        _passedScope.Should().NotBeNull();
        _passedScope!.OuterScope.Should().Be(_scope);
    }

    [Test]
    public void AVariableIsDefinedForEachArgumentPassedToTheCommandWithItsCorrespondingValue()
    {
        _userCommandVarArgs.Invoke(_scope, new ListValue(
            1.ToValue(), 2.ToValue(), 3.ToValue(), 4.ToValue(), 5.ToValue(), 6.ToValue()));
        
        _passedScope!.GetVariable("a").Should().Be(1.ToValue());
        _passedScope!.GetVariable("b").Should().Be(2.ToValue());
        _passedScope!.GetVariable("c").Should().Be(3.ToValue());
        _passedScope!.GetVariable("d").Should().Be(4.ToValue());
        _passedScope!.GetVariable("e").Should().Be(new ListValue(5.ToValue(), 6.ToValue()));
    }

    [Test]
    public void IfTooFewArgumentsArePassedAnErrorIsThrown()
    {
        _userCommand2Args.Invoking(c => c.Invoke(_scope, new ListValue(1.ToValue()))).Should()
            .Throw<ArgError>().WithMessage("Expected 'b' argument.");
    }

    [Test]
    public void IfTooManyArgumentsArePassedAnErrorIsThrown()
    {
        _userCommand2Args.Invoking(c => c.Invoke(_scope, new ListValue(1.ToValue(), 2.ToValue(), 3.ToValue()))).Should()
            .Throw<ArgError>().WithMessage("Unexpected argument '3'.");
    }

    [Test]
    public void IfACommandHasOptionalArgumentsThatAreNotSpecifiedTheyAreSetToTheirDefaultValues()
    {
        _userCommand2To4Args.Invoke(_scope, new ListValue(
            1.ToValue(), 2.ToValue()));
        
        _passedScope!.GetVariable("a").Should().Be(1.ToValue());
        _passedScope!.GetVariable("b").Should().Be(2.ToValue());
        _passedScope!.GetVariable("c").Should().Be(3.ToValue());
        _passedScope!.GetVariable("d").Should().Be(4.ToValue());
    }

    [SetUp]
    public void Setup()
    {
        _rootEvaluator = new Evaluator();

        _scope = new();
        var testCommand = new SimpleCommand((scope, _) => 
        {
            _passedScope = scope;
            return "test-command-result".ToValue(); 
        });
        _scope.DefineCommand("test", testCommand);
        
        _userCommandBody = Node.Command(Node.Literal("test"), new ListValue());

        _userCommandNoArgs = new UserCommand(
            Array.Empty<string>(), Array.Empty<(string, Value)>(), null, _userCommandBody);
        
        _userCommand2Args = new UserCommand(
            new string[] { "a", "b" }, Array.Empty<(string, Value)>(), null, _userCommandBody);
        
        _userCommand2To4Args = new UserCommand(
            new string[] { "a", "b" }, 
            new (string, Value)[] { ("c", 3.ToValue()), ("d", 4.ToValue()) }, 
            null, 
            _userCommandBody);
        
        _userCommandVarArgs = new UserCommand(
            new string[] { "a", "b" }, 
            new (string, Value)[] { ("c", 3.ToValue()), ("d", 4.ToValue()) }, 
            "e", 
            _userCommandBody);
    }
}