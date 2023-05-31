namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class UserCommandTests
{
    UserCommand _userCommandNoArgs = null!;
    UserCommand _userCommand2Args = null!;
    UserCommand _userCommand2To4Args = null!;
    UserCommand _userCommandVarArgs = null!;
    UserCommand _userCommandWithReturn = null!;

    Environment _env = null!;

    IEnvironment? _passedEnv = null;
    IScope? _passedScope = null;

    DictionaryValue _userCommandBody = null!;

    [Test]
    public void TheBodyIsEvaluatedTheResultIsReturned()
    {
        var result = _userCommandNoArgs.Invoke(_env, new ListValue());

        result.Should().Be("test-command-result".ToValue());
    }

    [Test]
    public void TheBodyIsEvaluatedInTheCurrentEnvironmentWithANewScopeWithinTheCurrentScope()
    {
        _userCommandNoArgs.Invoke(_env, new ListValue());

        _passedEnv.Should().BeSameAs(_env);
        _passedScope.Should().NotBeNull();
        _passedScope!.OuterScope.Should().Be(_env.GlobalScope);
    }

    [Test]
    public void AVariableIsDefinedForEachArgumentPassedToTheCommandWithItsCorrespondingEvaluatedValue()
    {
        _userCommandVarArgs.Invoke(_env, new ListValue(
            Node.Literal(1), Node.Literal(2), Node.Literal(3), Node.Literal(4), Node.Literal(5), Node.Literal(6)));

        _passedScope!.GetVariable("a").Should().Be(1.ToValue());
        _passedScope!.GetVariable("b").Should().Be(2.ToValue());
        _passedScope!.GetVariable("c").Should().Be(3.ToValue());
        _passedScope!.GetVariable("d").Should().Be(4.ToValue());
        _passedScope!.GetVariable("e").Should().Be(new ListValue(5.ToValue(), 6.ToValue()));
    }

    [Test]
    public void IfTooFewArgumentsArePassedAnErrorIsThrown()
    {
        _userCommand2Args.Invoking(c => c.Invoke(_env, new ListValue(Node.Literal(1)))).Should()
            .Throw<ArgError>().WithMessage("Expected 'b' argument.");
    }

    [Test]
    public void IfTooManyArgumentsArePassedAnErrorIsThrown()
    {
        _userCommand2Args.Invoking(c => c.Invoke(_env, new ListValue(Node.Literal(1), Node.Literal(2), Node.Literal(3))))
            .Should().Throw<ArgError>().WithMessage("Unexpected argument '3'.");
    }

    [Test]
    public void IfACommandHasOptionalArgumentsThatAreNotSpecifiedTheyAreSetToTheirDefaultValues()
    {
        _userCommand2To4Args.Invoke(_env, new ListValue(
            Node.Literal(1), Node.Literal(2)));

        _passedScope!.GetVariable("a").Should().Be(1.ToValue());
        _passedScope!.GetVariable("b").Should().Be(2.ToValue());
        _passedScope!.GetVariable("c").Should().Be(3.ToValue());
        _passedScope!.GetVariable("d").Should().Be(4.ToValue());
    }

    [Test]
    public void IfTheUserCommandThrowsAReturnTheReturnsValueIsReturned()
    {
        var result = _userCommandWithReturn.Invoke(_env, new ListValue());

        result.Should().Be("returnedValue".ToValue());
    }

    [Test]
    public void TheBodyOfTheCommandCanBeRetrieved()
    {
        _userCommandNoArgs.Body.Should().Be(_userCommandBody);
    }

    [Test]
    public void TheNamesOfTheRequiredArgumentCanBeRetrieved()
    {
        _userCommand2Args.RequiredArgNames.Should().Equal("a", "b");
    }

    [Test]
    public void TheNamesOfTheOptionalArgumentCanBeRetrieved()
    {
        _userCommand2Args.OptionalArgNames.Should().BeEmpty();
        _userCommand2To4Args.OptionalArgNames.Should().Equal("c", "d");
    }

    [Test]
    public void TheDefaultValuesOfTheOptionalArgumentCanBeRetrieved()
    {
        _userCommand2Args.OptionalArgDefaultValues.Should().BeEmpty();
        _userCommand2To4Args.OptionalArgDefaultValues.Should().Equal(3.ToValue(), 4.ToValue());
    }

    [Test]
    public void TheNameOfTheRestArgumentCanBeRetrieved()
    {
        _userCommandNoArgs.RestArgName.Should().BeNull();
        _userCommandVarArgs.RestArgName.Should().Be("e");
    }

    [SetUp]
    public void Setup()
    {
        _env = new();
        var testCommand = new SimpleCommand((env, _) =>
        {
            _passedEnv = env;
            _passedScope = env.CurrentScope;
            return "test-command-result".ToValue();
        });
        _env.GlobalScope.DefineCommand("test", testCommand);

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

        _userCommandWithReturn = new UserCommand(
            Array.Empty<string>(), Array.Empty<(string, Value)>(), null,
            Node.Raise(Node.Literal("/return"), Node.Literal(Value.Empty), Node.Literal("returnedValue")));
    }
}