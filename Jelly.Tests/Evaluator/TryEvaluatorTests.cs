namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class TryEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _evaluator = new TryEvaluator();
        _rootEvaluator = new Evaluator();
        _scope = new Scope();
    }

    [Test]
    public void TheResultOfANonThrowingTryBlockIsTheResultOfEvaluatingItsBody()
    {
        var tryNode = Node.Try(Node.Literal("success"), null);
        
        var result = _evaluator.Evaluate(_scope, tryNode, _rootEvaluator);

        result.Should().Be("success".ToValue());
    }

    [Test]
    public void TheFirstMatchingMatchingErrorHandlerBodyIsEvaluatedIfEvaluatingTheBodyThrowsAnErrorAndTheResultIsReturned()
    {
        var tryNode = Node.Try(
            Node.Raise(Node.Literal("/error/type"), Node.Literal("TypeError"), Node.Literal(Value.Empty)),
            null, 
            (Node.Literal("/error/"), Node.Literal("me first")),
            (Node.Literal("/error/type"), Node.Literal("even though I'm more specific")));
        
        var result = _evaluator.Evaluate(_scope, tryNode, _rootEvaluator);

        result.Should().Be("me first".ToValue());
    }

    [Test]
    public void IfNoneOfThePatternsMatchTheErrorIsRethrown()
    {
        var tryNode = Node.Try(
            Node.Raise(Node.Literal("/error"), Node.Literal("Test error"), Node.Literal("Test")),
            null, 
            (Node.Literal("/wont/"), Node.Literal("not going to happen")),
            (Node.Literal("/match"), Node.Literal("ever!")));
        
        _evaluator.Invoking(e => e.Evaluate(_scope, tryNode, _rootEvaluator)).Should()
            .Throw<Error>().WithMessage("Test error").Where(e => e.Value.Equals("Test".ToValue()));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void IfAFinallyBodyIsProvidedItIsEvaluatedAndItsResultIsReturnedWeatherAnErrorIsRaisedAndExcpetedOrNot(bool raiseError)
    {
        var tryNode = Node.Try(
            raiseError
                ? Node.Raise(Node.Literal("/error/type"), Node.Literal("TypeError"), Node.Literal(Value.Empty))
                : Node.Literal("success"),
            Node.Literal("this should be the result"), 
            (Node.Literal("/error/"), Node.Literal("me first")),
            (Node.Literal("/error/type"), Node.Literal("even though I'm more specific")));
        
        var result = _evaluator.Evaluate(_scope, tryNode, _rootEvaluator);

        result.Should().Be("this should be the result".ToValue());
    }

    // TODO:  Add errtype, errmessage, errpos special variables to handler bodies.
}