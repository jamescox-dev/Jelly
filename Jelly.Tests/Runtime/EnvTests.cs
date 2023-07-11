namespace Jelly.Runtime.Tests;

using Jelly.Evaluator;

[TestFixture]
public class EnvTests
{
    IEnv _environment = null!;

    DictionaryValue _testNode = null!;

    Mock<IParser> _mockParser = null!;
    Mock<IEvaluator> _mockEvaluator = null!;

    [Test]
    public void TheEnvironmentIsConstructedWithAGlobalScope()
    {
        _environment.GlobalScope.Should().NotBeNull();
    }

    [Test]
    public void TheEnvironmentCanBeConstructedWithAUserSpecifiedGlobalScope()
    {
        var scope = new Scope();

        var env = new Env(scope);

        env.GlobalScope.Should().Be(scope);
    }

    [Test]
    public void TheDefaultCurrentScopeIsTheGlobalScope()
    {
        _environment.CurrentScope.Should().BeSameAs(_environment.GlobalScope);
    }

    [Test]
    public void TheEnvironmentIsConstructedWithAScriptParser()
    {
        var env = new Env();

        env.Parser.Should().BeOfType<ScriptParser>();
    }

    [Test]
    public void TheEnvironmentIsConstructedWithAEvaluator()
    {
        var env = new Env();

        env.Evaluator.Should().BeOfType<Evaluator>();
    }

    [Test]
    public void PushingAScopeSetsTheCurrentScopeToANewScopeWithTheOuterScopeSetToThePreviousCurrentScopeAndTheNewScopeIsReturned()
    {
        var previousCurrentScope = _environment.CurrentScope;

        var scope = _environment.PushScope();

        scope.Should().BeSameAs(_environment.CurrentScope);
        _environment.CurrentScope.OuterScope.Should().BeSameAs(previousCurrentScope);
    }

    [Test]
    public void PoppingAScopeSetsTheCurrentScopeToItsOuterScopeAndReturnsTheNewCurrentScope()
    {
        _environment.PushScope();

        var scope = _environment.PopScope();

        scope.Should().BeSameAs(_environment.CurrentScope);
        _environment.CurrentScope.Should().BeSameAs(_environment.GlobalScope);
    }

    [Test]
    public void PoppingTheGlobalScopeResultsInAnError()
    {
        // TODO:  Should throw StackUnderflowError
        _environment.Invoking(e => e.PopScope())
            .Should().Throw<Error>().WithMessage("Stack underflow.");
    }

    [Test]
    public void ANodeIsEvaluatedByTheEnvironmentsEvaluator()
    {
        var result = _environment.Evaluate(_testNode);

        result.Should().Be("RESULT!".ToValue());
    }

    [Test]
    public void AStringIsFirstParsedByTheEnvironmentsParserAndThenPassedToItsEvaluator()
    {
        var result = _environment.Evaluate("test");

        result.Should().Be("RESULT!".ToValue());
    }

    [Test]
    public void IfTheStringParsedDoesNotYieldANodeAEmptyValueIsReturnedWhenEvaluatingAString()
    {
        var result = _environment.Evaluate("  ");

        result.Should().Be(Value.Empty);
    }

    [SetUp]
    public void Setup()
    {
        _testNode = Node.Literal("test");

        _mockParser = new();
        _mockEvaluator = new();

        _environment = new Env(_mockParser.Object, _mockEvaluator.Object);

        _mockParser.Setup(m => m.Parse(It.Is<Scanner>(s => s.Source == "test"))).Returns(_testNode);

        _mockEvaluator.Setup(m => m.Evaluate(_environment, _testNode))
            .Returns("RESULT!".ToValue());
    }
}