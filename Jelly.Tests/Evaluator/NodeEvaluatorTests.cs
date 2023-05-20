namespace Jelly.Evaluator.Tests;

[TestFixture]
public class NodeEvaluatorTests
{
    [Test]
    public void TheNodeEvaluatorCallsTheConfiguredEvaluatorForTheGivenNodeTypeAndTheScopeAndEvaluatorPassedToEvaluateIsPassedToTheRetrievedEvaluatorsEvaluateMethod()
    {
        var scope = new Scope();
        var anotherInterpreter = new TestEvaluator();
        var interpreter = new NodeEvaluator();
        var test1Interpreter = new TestEvaluator();
        var test2Interpreter = new TestEvaluator();
        interpreter.AddEvaluator("type1", test1Interpreter);
        interpreter.AddEvaluator("type2", test2Interpreter);
        var test1Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type1")),
            new(new StringValue("message"), new StringValue("hi")),
        });
        var test2Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type2")),
            new(new StringValue("message"), new StringValue("bye")),
        });

        var test1result = interpreter.Evaluate(scope, test1Node, anotherInterpreter);
        var test2result = interpreter.Evaluate(scope, test2Node, anotherInterpreter);

        test1result.Should().Be(new StringValue("hi"));
        test1Interpreter.ScopePassedToEvaluate.Should().Be(scope);
        test1Interpreter.NodeEvaluated.Should().Be(test1Node);
        test1Interpreter.EvaluatorPassedToEvaluate.Should().Be(anotherInterpreter);
        test2result.Should().Be(new StringValue("bye"));
        test2Interpreter.ScopePassedToEvaluate.Should().Be(scope);
        test2Interpreter.NodeEvaluated.Should().Be(test2Node);
        test2Interpreter.EvaluatorPassedToEvaluate.Should().Be(anotherInterpreter);
    }

    [Test]
    public void WhenTheTypeOfTheNodeIsMissingAnErrorIsThrown()
    {
        IEvaluator evaluator = new Evaluator();
        var invalidNode = new DictionaryValue();

        evaluator.Invoking(e => e.Evaluate(new Mock<IScope>().Object, invalidNode))
            .Should().Throw<Error>().WithMessage("Can not evaluate node, no type specified.")
            .Where(e => e.Type == "/error/eval/");
    }

    [Test]
    public void WhenTheTypeOfNodeIsNotKnownToTheEvaluatorAEvaluationErrorIsThrown()
    {
        IEvaluator evaluator = new Evaluator();
        var invalidNode = new DictionaryValue(
            "type".ToValue(), "invalid".ToValue()
        );

        evaluator.Invoking(e => e.Evaluate(new Mock<IScope>().Object, invalidNode))
            .Should().Throw<Error>().WithMessage("Can not evaluate node of type: 'invalid'.")
            .Where(e => e.Type == "/error/eval/");
    }

    public class TestEvaluator : IEvaluator
    {
        public IScope? ScopePassedToEvaluate { get; private set; }
        public DictionaryValue? NodeEvaluated { get; private set; }
        public IEvaluator? EvaluatorPassedToEvaluate { get; private set; }

        public Value Evaluate(IScope scope, DictionaryValue node, IEvaluator evaluator)
        {
            ScopePassedToEvaluate = scope;
            NodeEvaluated = node;
            EvaluatorPassedToEvaluate = evaluator;

            return node[new StringValue("message")];
        }
    }
}