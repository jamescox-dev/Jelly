namespace Jelly.Evaluator.Tests;

[TestFixture]
public class NodeEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheNodeEvaluatorCallsTheConfiguredEvaluatorForTheGivenNodeTypeAndTheScopeAndEvaluatorPassedToEvaluateIsPassedToTheRetrievedEvaluatorsEvaluateMethod()
    {
        var nodeEvaluator = (NodeEvaluator)Evaluator;
        var test1Interpreter = new TestEvaluator();
        var test2Interpreter = new TestEvaluator();
        nodeEvaluator.AddEvaluator("type1", test1Interpreter);
        nodeEvaluator.AddEvaluator("type2", test2Interpreter);
        var test1Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type1")),
            new(new StringValue("message"), new StringValue("hi")),
        });
        var test2Node = new DictionaryValue(new KeyValuePair<Value, Value>[] {
            new(new StringValue("type"), new StringValue("type2")),
            new(new StringValue("message"), new StringValue("bye")),
        });

        var test1result = nodeEvaluator.Evaluate(Environment, test1Node);
        var test2result = nodeEvaluator.Evaluate(Environment, test2Node);

        test1result.Should().Be(new StringValue("hi"));
        test1Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test1Interpreter.NodeEvaluated.Should().Be(test1Node);
        test1Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test2result.Should().Be(new StringValue("bye"));
        test2Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test2Interpreter.NodeEvaluated.Should().Be(test2Node);
        test2Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
    }

    [Test]
    public void WhenTheTypeOfTheNodeIsMissingAnErrorIsThrown()
    {
        IEvaluator evaluator = new Evaluator();
        var invalidNode = new DictionaryValue();

        this.Invoking(e => e.Evaluate(invalidNode))
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

        this.Invoking(e => e.Evaluate(invalidNode))
            .Should().Throw<Error>().WithMessage("Can not evaluate node of type: 'invalid'.")
            .Where(e => e.Type == "/error/eval/");
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new NodeEvaluator();
    }

    public class TestEvaluator : IEvaluator
    {
        public IEnv? EnvironmentPassedToEvaluate { get; private set; }
        public DictionaryValue? NodeEvaluated { get; private set; }
        public IEvaluator? EvaluatorPassedToEvaluate { get; private set; }

        public Value Evaluate(IEnv env, DictionaryValue node)
        {
            EnvironmentPassedToEvaluate = env;
            NodeEvaluated = node;

            return node[new StringValue("message")];
        }
    }
}