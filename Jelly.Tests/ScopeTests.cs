namespace Jelly.Tests;

using Jelly.Values;

[TestFixture]
public class ScopeTests
{
    [Test]
    public void AVariableCanBeDefinedInTheScopeAndItsValueRetrieved()
    {
        var scope = new Scope();

        scope.DefineVariable("name", new StringValue("Bob"));
        var value = scope.GetVariable("name");

        value.Should().Be(new StringValue("Bob"));
    }
}