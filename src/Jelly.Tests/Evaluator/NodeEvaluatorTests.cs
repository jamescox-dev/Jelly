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
        var test1Node = new DictValue(new KeyValuePair<Value, Value>[] {
            new(new StrValue("type"), new StrValue("type1")),
            new(new StrValue("message"), new StrValue("hi")),
        });
        var test2Node = new DictValue(new KeyValuePair<Value, Value>[] {
            new(new StrValue("type"), new StrValue("type2")),
            new(new StrValue("message"), new StrValue("bye")),
        });

        var test1result = nodeEvaluator.Evaluate(Environment, test1Node);
        var test2result = nodeEvaluator.Evaluate(Environment, test2Node);

        test1result.Should().Be(new StrValue("hi"));
        test1Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test1Interpreter.NodeEvaluated.Should().Be(test1Node);
        test1Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test2result.Should().Be(new StrValue("bye"));
        test2Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
        test2Interpreter.NodeEvaluated.Should().Be(test2Node);
        test2Interpreter.EnvironmentPassedToEvaluate.Should().Be(Environment);
    }

    [Test]
    public void WhenTheTypeOfTheNodeIsMissingAnErrorIsThrown()
    {
        IEvaluator evaluator = new Evaluator();
        var invalidNode = new DictValue();

        this.Invoking(e => e.Evaluate(invalidNode))
            .Should().Throw<Error>().WithMessage("Can not evaluate node, no type specified.")
            .Where(e => e.Type == "/error/eval/");
    }

    [Test]
    public void WhenTheTypeOfNodeIsNotKnownToTheEvaluatorAEvaluationErrorIsThrown()
    {
        IEvaluator evaluator = new Evaluator();
        var invalidNode = new DictValue(
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
        public DictValue? NodeEvaluated { get; private set; }
        public IEvaluator? EvaluatorPassedToEvaluate { get; private set; }

        public Value Evaluate(IEnv env, DictValue node)
        {
            EnvironmentPassedToEvaluate = env;
            NodeEvaluated = node;

            return node[new StrValue("message")];
        }
    }
}