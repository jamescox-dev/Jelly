namespace Jelly.Evaluator;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class RaiseEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;

    [Test]
    public void AErrorIsThrownOfTheCorrectTypeAndWithTheCorrectMessageAndValue()
    {
        var node = Node.Raise(Node.Literal("/error/test"), Node.Literal("Test message."), Node.Literal("testvalue"));

        _evaluator.Invoking(e => e.Evaluate(new Mock<IScope>().Object, node, _rootEvaluator)).Should()
            .Throw<Error>().WithMessage("Test message.").Where(e => e.Value.Equals("testvalue".ToValue()));
    }

    [SetUp]
    public void Setup()
    {
        _evaluator = new RaiseEvaluator();
        _rootEvaluator = new Evaluator();
    }
}