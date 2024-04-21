namespace Jelly.Tests.Runtime;

[TestFixture]
public class EnvTests
{
    IEnv _environment = null!;

    DictValue _testNode = null!;
    DictValue _errorTestNode = null!;
    DictValue _clrErrorTestNode = null!;

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

        env.Evaluator.Should().BeOfType<Jelly.Evaluator.Evaluator>();
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

    [Test]
    public void WhenEvaluatingAScriptRaisesAnErrorWithoutAPositionThePositionOfTheNodeIsAttachToTheError()
    {
        _environment.Invoking(e => e.Evaluate("boo!"))
            .Should().Throw<Error>().Where(e =>
                e.Type == "/test/"
                && e.Message == "Boo!"
                && e.StartPosition == 1
                && e.EndPosition == 5);
    }

    [Test]
    public void WhenEvaluatingAScriptRaisesAClrExceptionItIsTranslatedToAJellyErrorWithThePositionOfTheNode()
    {
        _environment.Invoking(e => e.Evaluate("sys"))
            .Should().Throw<Error>().Where(e =>
                e.Type == "/wobbly/exception/"
                && e.Message == "CLR Error"
                && e.StartPosition == 10
                && e.EndPosition == 50);
    }

    [SetUp]
    public void Setup()
    {
        _testNode = Node.Literal("test");
        _errorTestNode = Node.Literal(1, 5, "boo!");
        _clrErrorTestNode = Node.Literal(10, 50, "sys");

        _mockParser = new();
        _mockEvaluator = new();

        _environment = new Env(_mockParser.Object, _mockEvaluator.Object);

        _mockParser.Setup(m => m.Parse(It.Is<Scanner>(s => s.Source == "test"))).Returns(_testNode);
        _mockParser.Setup(m => m.Parse(It.Is<Scanner>(s => s.Source == "boo!"))).Returns(_errorTestNode);
        _mockParser.Setup(m => m.Parse(It.Is<Scanner>(s => s.Source == "sys"))).Returns(_clrErrorTestNode);

        _mockEvaluator.Setup(m => m.Evaluate(_environment, _testNode))
            .Returns("RESULT!".ToValue());

        _mockEvaluator.Setup(m => m.Evaluate(_environment, _errorTestNode))
            .Throws(new Error("/test/", "Boo!"));

        _mockEvaluator.Setup(m => m.Evaluate(_environment, _clrErrorTestNode))
            .Throws(new Exception("CLR Error"));
    }

    [Test]
    public void TheOnEvaluateHookIsCalledForEachNodeEvaluated()
    {
        var evaluatedNodes = new List<DictValue>();
        var onEvaluate = (DictValue node) => { evaluatedNodes.Add(node); };
        var env = new Env(new EnvHooks(onEvaluate));
        var a = Node.Literal(1);
        var b = Node.Literal(2);
        var binOp = Node.BinOp("add", a, b);
        var expr = Node.Expression(binOp);

        env.Evaluate(expr);

        evaluatedNodes.Should().HaveCount(4);
        evaluatedNodes[0].Should().Be(expr);
        evaluatedNodes[1].Should().Be(binOp);
        evaluatedNodes[2].Should().Be(a);
        evaluatedNodes[3].Should().Be(b);
    }
}