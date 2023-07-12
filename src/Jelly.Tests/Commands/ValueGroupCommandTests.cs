namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class ValueGroupCommandTests
{
    IEnv _env = null!;

    [SetUp]
    public void Setup()
    {
        _env = new Env();
    }

    [Test]
    public void IfTheValueGroupCommandDoesNotReceiveAValueAnErrorIsThrown()
    {
        var group = new ValueGroupCommand("test1", "thing");

        group.Invoking(g => g.Invoke(_env, ListValue.EmptyList))
            .Should().Throw<MissingArgError>().WithMessage("test1 missing 1 required argument:  thing.");
    }

    [Test]
    public void IfTheValueGroupCommandDoesNotReceiveASubCommandNameAnErrorIsThrown()
    {
        var group = new ValueGroupCommand("test", "thing");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("1"))))
            .Should().Throw<MissingArgError>().WithMessage("test missing 1 required argument:  command.");
    }

    [Test]
    public void WhenASubCommandIsSpecifiedItIsCalledWithValueAndTheRestOfTheArgumentsAndTheReturnValueIsReturned()
    {
        var subCommand = new TestCommand
        {
            ReturnValue = "z".ToValue()
        };
        var group = new ValueGroupCommand("test2", "widget");
        group.AddCommand("sub1", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Literal("value"), Node.Literal("SUB1"), "a".ToValue(), "b".ToValue()));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("value"), "a".ToValue(), "b".ToValue()));
        result.Should().Be("z".ToValue());
    }

    [Test]
    public void IfTheValueGroupCommandDoesNotReceiveASubCommandNameButADefaultCommandNameIsConfiguredThatCommandIsCalled()
    {
        var subCommand = new TestCommand
        {
            ReturnValue = "x".ToValue()
        };
        var group = new ValueGroupCommand("test2", "widget", "sub2");
        group.AddCommand("Sub2", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Literal("value")));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("value")));
        result.Should().Be("x".ToValue());
    }

    [Test]
    public void IfTheSubCommandDoesNotExistAnErrorIsThrown()
    {
        var group = new ValueGroupCommand("test1", "stuff");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("hey"), Node.Literal("Not Here"))))
            .Should().Throw<NameError>().WithMessage("Unknown sub-command 'Not Here'.");
    }
}