namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class GroupCommandTests
{
    IEnv _env = null!;

    [SetUp]
    public void Setup()
    {
        _env = new Env();
    }

    [Test]
    public void IfTheGroupCommandDoesNotReceiveAValueAnErrorIsThrown()
    {
        var group = new GroupCommand("test1");

        group.Invoking(g => g.Invoke(_env, ListValue.EmptyList))
            .Should().Throw<MissingArgError>().WithMessage("test1 missing 1 required argument:  command.");
    }

    [Test]
    public void WhenASubCommandIsSpecifiedItIsCalledWithTheRestOfTheArgumentsAndTheReturnValueIsReturned()
    {
        var subCommand = new TestCommand
        {
            ReturnValue = "z".ToValue()
        };
        var group = new GroupCommand("test2");
        group.AddCommand("sub1", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Literal("SUB1"), "a".ToValue(), "b".ToValue()));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue("a".ToValue(), "b".ToValue()));
        result.Should().Be("z".ToValue());
    }

    [Test]
    public void IfTheSubCommandDoesNotExistAnErrorIsThrown()
    {
        var group = new GroupCommand("test1");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("404"))))
            .Should().Throw<NameError>().WithMessage("Unknown sub-command '404'.");
    }
}