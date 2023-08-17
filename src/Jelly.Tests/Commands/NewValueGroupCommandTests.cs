namespace Jelly.Commands.Tests;

// TODO:  Rename this to ValueGroupCommandTests, and remove old ValueGroupCommandTest & VariableTestsGroupCommand.
[TestFixture]
public class NewValueGroupCommandTests
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
        var group = new NewValueGroupCommand("test1", "thing");

        group.Invoking(g => g.Invoke(_env, ListValue.EmptyList))
            .Should().Throw<MissingArgError>().WithMessage("test1 missing 1 required argument:  thing.");
    }

    [Test]
    public void IfTheValueGroupCommandDoesNotReceiveASubCommandNameAnErrorIsThrown()
    {
        var group = new NewValueGroupCommand("test", "thing");

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
        var group = new NewValueGroupCommand("test2", "widget");
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
        var group = new NewValueGroupCommand("test2", "widget", "sub2");
        group.AddCommand("Sub2", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Literal("value")));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("value")));
        result.Should().Be("x".ToValue());
    }

    [Test]
    public void IfTheSubCommandDoesNotExistAnErrorIsThrown()
    {
        var group = new NewValueGroupCommand("test1", "stuff");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("hey"), Node.Literal("Not Here"))))
            .Should().Throw<NameError>().WithMessage("Unknown sub-command 'Not Here'.");
    }

    [Test]
    public void WhenTheSubCommandIsTheEqualsSignAndNoMutatorCommandsAreDefinedAnErrorIsThrown()
    {
        var group = new NewValueGroupCommand("test1", "stuff");

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("var"), Node.Literal("="))))
            .Should().Throw<NameError>().WithMessage("Unknown sub-command '='.");
    }

    [Test]
    public void WhenTheSubCommandIsTheEqualsSignAndAMutatorCommandIsDefinedButNoMutatorSubCommandIsGivenAnErrorIsThrown()
    {
        var group = new NewValueGroupCommand("test1", "stuff");

        group.AddMutatorCommand("mutant", new TestCommand());

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("var_a"), Node.Literal("="))))
            .Should().Throw<MissingArgError>().WithMessage("test1 missing 1 required argument:  command.");
    }

    [Test]
    public void WhenTheSubCommandIsTheEqualsSignAndAMutatorCommandIsDefinedButTheSubCommandIsNotAMutatorCommandAnErrorIsThrown()
    {
        var group = new NewValueGroupCommand("test1", "stuff");

        group.AddMutatorCommand("mutant", new TestCommand());
        group.AddCommand("not_a_mutant", new TestCommand());

        group.Invoking(g => g.Invoke(_env, new ListValue(Node.Literal("var_b"), Node.Literal("="), Node.Literal("not_a_mutant"))))
            .Should().Throw<ArgError>().WithMessage("Sub-command 'not_a_mutant' can not be used in assignment.");
    }

    [Test]
    public void WhenTheSubCommandIsTheEqualsSignAndAMutatorCommandIsGivenTheCommandIsCalledWithTheCurrentValueOfNamedVariableAsALiteralAndItsReturnValueIsAssignedToTheNamedVariable()
    {
        _env.GlobalScope.DefineVariable("a", "1 2".ToValue());
        var subCommand = new TestCommand
        {
            ReturnValue = "1 2 3".ToValue()
        };
        var group = new NewValueGroupCommand("list", "var");
        group.AddMutatorCommand("Mut", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Literal("a"), Node.Literal("="), Node.Literal("MUT"), Node.Literal("b")));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("1 2"), Node.Literal("b")));
        result.Should().Be("1 2 3".ToValue());
        _env.GlobalScope.GetVariable("a").Should().Be("1 2 3".ToValue());
    }

    [Test]
    public void WhenCallingAMutatorSubCommandTheVariableNameCanBeGivenAsAVariableNode()
    {
        _env.GlobalScope.DefineVariable("b", "1 2".ToValue());
        var subCommand = new TestCommand
        {
            ReturnValue = "1 2 3".ToValue()
        };
        var group = new NewValueGroupCommand("list", "var");
        group.AddMutatorCommand("Mut2", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Variable("b"), Node.Literal("="), Node.Literal("MUT2"), Node.Literal("x"), Node.Literal("y")));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("1 2"), Node.Literal("x"), Node.Literal("y")));
        result.Should().Be("1 2 3".ToValue());
        _env.GlobalScope.GetVariable("b").Should().Be("1 2 3".ToValue());
    }

    [Test]
    public void WhenCallingAMutatorSubCommandWithAVariableNodeIndexersAreSupported()
    {
        _env.GlobalScope.DefineVariable("l", new ListValue("a".ToValue(), "b".ToValue()));
        var subCommand = new TestCommand
        {
            ReturnValue = "x".ToValue()
        };
        var group = new NewValueGroupCommand("list", "var");
        group.AddMutatorCommand("Mut3", subCommand);

        var result = group.Invoke(_env, new ListValue(Node.Variable(0, 0, "l", Node.ListIndexer(0, 0, Node.Literal(2))), Node.Literal("="), Node.Literal("mut3")));

        ((Value?)subCommand.ArgsPassedToInvoke).Should().Be(new ListValue(Node.Literal("b".ToValue())));
        result.Should().Be("x".ToValue());
        _env.GlobalScope.GetVariable("l").Should().Be(new ListValue("a".ToValue(), "x".ToValue()));
    }
}