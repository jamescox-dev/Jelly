namespace Jelly.Tests;

using Jelly.Commands;
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
    public void ACommandCanBeDefinedInAScopeAndRetrievedByItsName()
    {
        var scope = new Scope();
        var testCommand = new Mock<ICommand>();

        scope.DefineCommand("test", testCommand.Object);
        var command = scope.GetCommand("test");

        command.Should().Be(testCommand.Object);
    }
}