namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class VariableGroupCommandTests
{
    IEnv _env = null!;

    [SetUp]
    public void Setup()
    {
        _env = new Env();
    }

    [Test]
    public void IfTheVariableGroupCommandDoesNotReceiveAValueAnErrorIsThrown()
    {
        var group = new VariableGroupCommand("test1");

        group.Invoking(g => g.Invoke(_env, ListValue.EmptyList))
            .Should().Throw<MissingArgError>().WithMessage("test1 missing 1 required argument:  variable.");
    }

    [Test]
    public void IfTheVariableGroupCommandDoesNotReceiveASubCommandNameAnErrorIsThrown()
    {
        var group = new VariableGroupCommand("test");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Variable("a"))))
            .Should().Throw<MissingArgError>().WithMessage("test missing 1 required argument:  command.");
    }

    [Test]
    public void WhenASubCommandIsSpecifiedItIsCalledWithValueOfTheVariableAsALiteralAndTheRestOfTheArgumentsAndTheReturnValueIsReturnedAndAssignedBackToTheVariable()
    {
        _env.GlobalScope.DefineVariable("var", "1".ToValue());
        var subCommand = new TestCommand
        {
            ReturnValue = "z".ToValue()
        };
        var group = new VariableGroupCommand("test2");
        group.AddCommand("sub3", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Variable("var"), Node.Literal("SUb3"), "a".ToValue(), "b".ToValue()));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("1"), "a".ToValue(), "b".ToValue()));
        result.Should().Be("z".ToValue());
        _env.GlobalScope.GetVariable("var").Should().Be("z".ToValue());
    }

    [Test]
    public void IfTheSubCommandDoesNotExistAnErrorIsThrown()
    {
        var group = new VariableGroupCommand("test1");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("hey"), Node.Literal("Go Away!"))))
            .Should().Throw<NameError>().WithMessage("Unknown sub-command 'Go Away!'.");
    }
}