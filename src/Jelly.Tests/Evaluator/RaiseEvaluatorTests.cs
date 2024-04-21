namespace Jelly.Tests.Evaluator;

[TestFixture]
public class RaiseEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void AErrorIsThrownOfTheCorrectTypeAndWithTheCorrectMessageAndValue()
    {
        var node = Node.Raise(Node.Literal("error"), Node.Literal("Test message."), Node.Literal("test-value"));

        Evaluator.Invoking(e => e.Evaluate(Environment, node)).Should()
            .Throw<Error>()
            .WithMessage("Test message.")
            .Where(e => e.Value.Equals("test-value".ToValue()))
            .Where(e => e.Type == Error.NormalizeType(e.Type));
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new RaiseEvaluator();
    }
}