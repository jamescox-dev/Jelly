namespace Jelly.Tests.Evaluator;

[TestFixture]
public class TryEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheResultOfANonThrowingTryBlockIsTheResultOfEvaluatingItsBody()
    {
        var tryNode = Node.Try(Node.Literal("success"), null);

        var result = Evaluate(tryNode);

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

        var result = Evaluate(tryNode);

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

        this.Invoking(e => e.Evaluate(tryNode)).Should()
            .Throw<Error>().WithMessage("Test error").Where(e => e.Value.Equals("Test".ToValue()));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void IfAFinallyBodyIsProvidedItIsEvaluatedAndItsResultIsReturnedWeatherAnErrorIsRaisedAndExceptedOrNot(bool raiseError)
    {
        var tryNode = Node.Try(
            raiseError
                ? Node.Raise(Node.Literal("/error/type"), Node.Literal("TypeError"), Node.Literal(Value.Empty))
                : Node.Literal("success"),
            Node.Literal("this should be the result"),
            (Node.Literal("/error/"), Node.Literal("me first")),
            (Node.Literal("/error/type"), Node.Literal("even though I'm more specific")));

        var result = Evaluate(tryNode);

        result.Should().Be("this should be the result".ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new TryEvaluator();
    }

    // TODO:  Add errtype, errmessage, errpos special variables to handler bodies.
}