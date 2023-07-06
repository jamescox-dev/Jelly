namespace Jelly.Library.Tests;

using Jelly.Runtime;

[TestFixture]
public class MathLibraryTests
{
    ILibrary _lib = null!;
    Environment _env = null!;

    [SetUp]
    public virtual void Setup()
    {
        _lib = new MathLibrary();
        _env = new();

        _lib.LoadIntoScope(_env.GlobalScope);
    }

    [TestFixture]
    public class WhenLoadedIntoAScope : MathLibraryTests
    {
        [Test]
        public void TheScopeHasTheCorrectCommandsDefined()
        {
            _env.GlobalScope.Invoking(s => s.GetCommand("math")).Should().NotThrow();
            var mathCmd = (GroupCommand)_env.GlobalScope.GetCommand("math");
            mathCmd.Invoking(g => g.GetCommand("max")).Should().NotThrow();
            mathCmd.Invoking(g => g.GetCommand("min")).Should().NotThrow();
        }

        [Test]
        public void TheScopeHasTheCorrectVariablesDefined()
        {
            _env.GlobalScope.GetVariable("PI").Should().Be(Math.PI.ToValue());
            _env.GlobalScope.GetVariable("TAU").Should().Be(Math.Tau.ToValue());
        }
    }

    #region max

    [TestFixture]
    public class MaxTests : MathLibraryTests
    {
        [Test]
        public void WhenCalledWithNoArgumentsReturnsZero()
        {
            var maxCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("max");

            var result = maxCmd.Invoke(_env, new ListValue());

            result.Should().Be(NumberValue.Zero);
        }

        [Test]
        public void TheMaximumValueOfTheArgumentsIsReturned()
        {
            var maxCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("max");

            var result = maxCmd.Invoke(_env, new ListValue(Node.Literal(-1), Node.Literal(0), Node.Literal(10)));

            result.Should().Be(10.ToValue());
        }

        [Test]
        public void WhenAnyOfTheArgumentsAreNaNTheResultIsNaN()
        {
            var maxCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("max");

            var result = maxCmd.Invoke(_env, new ListValue(Node.Literal(-1), Node.Literal("Boo!"), Node.Literal(10)));

            result.Should().Be(NumberValue.NaN);
        }
    }

    #endregion

        #region max

    [TestFixture]
    public class MinTests : MathLibraryTests
    {
        [Test]
        public void WhenCalledWithNoArgumentsReturnsZero()
        {
            var minCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("min");

            var result = minCmd.Invoke(_env, new ListValue());

            result.Should().Be(NumberValue.Zero);
        }

        [Test]
        public void TheMinimumValueOfTheArgumentsIsReturned()
        {
            var minCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("min");

            var result = minCmd.Invoke(_env, new ListValue(Node.Literal(-1), Node.Literal(0), Node.Literal(10)));

            result.Should().Be((-1).ToValue());
        }

        [Test]
        public void WhenAnyOfTheArgumentsAreNaNTheResultIsNaN()
        {
            var minCmd = ((GroupCommand)_env.GlobalScope.GetCommand("math")).GetCommand("min");

            var result = minCmd.Invoke(_env, new ListValue(Node.Literal(-1), Node.Literal("Boo!"), Node.Literal(10)));

            result.Should().Be(NumberValue.NaN);
        }
    }

    #endregion
}