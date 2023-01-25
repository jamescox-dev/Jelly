namespace Jelly.Tests;

using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class ScopeTests
{
    [Test]
    public void AVariableCanBeDefinedInAScopeAndItsValueRetrieved()
    {
        var scope = new Scope();

        scope.DefineVariable("name", "Bob".ToValue());
        var value = scope.GetVariable("name");

        value.Should().Be("Bob".ToValue());
    }

    [Test]
    public void AVariableCanBeRedefinedInAScopeAndItsUpdatedValueRetrieved()
    {
        var scope = new Scope();

        scope.DefineVariable("name", "Vic".ToValue());
        scope.DefineVariable("name", "Bob".ToValue());
        var value = scope.GetVariable("name");

        value.Should().Be("Bob".ToValue());
    }

    [Test]
    public void AVariablesNameIsNotCaseSensitive()
    {
        var scope = new Scope();

        scope.DefineVariable("Album", "Abbey Road".ToValue());
        var value = scope.GetVariable("aLBUM");

        value.Should().Be("Abbey Road".ToValue());
    }

    [Test]
    public void AVariableCanSetInAScope()
    {
        var scope = new Scope();

        scope.DefineVariable("name", "Vic".ToValue());
        scope.SetVariable("name", "Bob".ToValue());
        
        var value = scope.GetVariable("name");

        value.Should().Be("Bob".ToValue());
    }

    [Test]
    public void TryingToGetAVariableThatDoesNotExistsThrowsAnError()
    {
        var scope = new Scope();

        scope.Invoking(s => s.GetVariable("unknown")).Should()
            .Throw<NameError>().WithMessage("Variable 'unknown' not defined.");
    }
    
    [Test]
    public void TryingToSetAVariableThatDoesNotExistsThrowsAnError()
    {
        var scope = new Scope();

        scope.Invoking(s => s.SetVariable("unknown", Value.Empty)).Should()
            .Throw<NameError>().WithMessage("Variable 'unknown' not defined.");
    }
    
    [Test]
    public void ACommandCanBeDefinedInAScopeAndRetrievedByItsName()
    {
        var scope = new Scope();
        var testCommand = new Mock<ICommand>();

        scope.DefineCommand("test", testCommand.Object);
        var command = scope.GetCommand("test");

        command.Should().Be(testCommand.Object);
    }

    [Test]
    public void ACommandCanBeRedefinedInAScopeAndRetrievedByItsName()
    {
        var scope = new Scope();
        var testCommand1 = new Mock<ICommand>();
        var testCommand2 = new Mock<ICommand>();

        scope.DefineCommand("test", testCommand1.Object);
        scope.DefineCommand("test", testCommand2.Object);
        var command = scope.GetCommand("test");

        command.Should().Be(testCommand2.Object);
    }

    [Test]
    public void ACommandNameIsNotCaseSensitive()
    {
        var scope = new Scope();
        var testCommand = new Mock<ICommand>();

        scope.DefineCommand("Case", testCommand.Object);
        var command = scope.GetCommand("cASE");

        command.Should().Be(testCommand.Object);
    }

    [Test]
    public void IfACommandDoesNotExistAnErrorIsThrown()
    {
        var scope = new Scope();

        scope.Invoking(s => s.GetCommand("test")).Should()
            .Throw<NameError>().WithMessage("Unknown command 'test'.");
    }
}