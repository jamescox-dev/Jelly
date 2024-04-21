namespace Jelly.Tests.Evaluator;

[TestFixture]
public class VariableEvaluatorTests
{
    [Test]
    public void EvaluatingAVariableNodeReturnsTheValueOfTheNamedVariableFromTheGivenScope()
    {
        var env = new Env();
        var evaluator = new VariableEvaluator();
        env.GlobalScope.DefineVariable("answer", "42".ToValue());
        var variable = Node.Variable("answer");

        var result = evaluator.Evaluate(env, variable);

        result.Should().Be(new StrValue("42"));
    }

    [Test]
    public void EvaluatingAVariableNodeWithAListIndexerEvaluatesItsExpressionAndUsesItAsAnIndexIntoTheListValueReturnedFromTheVariable()
    {
        var env = new Env();
        var evaluator = new VariableEvaluator();
        env.GlobalScope.DefineVariable("list", new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue()));
        var variable = Node.Variable(0, 0, "list", Node.ListIndexer(0, 0, Node.Literal(3)));

        var result = evaluator.Evaluate(env, variable);

        result.Should().Be("c".ToValue());
    }

    [Test]
    public void EvaluatingAVariableNodeWithADictIndexerEvaluatesItsExpressionAndUsesItAsAKeyIntoTheDictValueReturnedFromTheVariable()
    {
        var env = new Env();
        var evaluator = new VariableEvaluator();
        env.GlobalScope.DefineVariable("dict", new DictValue("x".ToValue(), 1.ToValue(), "y".ToValue(), 2.ToValue()));
        var variable = Node.Variable(0, 0, "dict", Node.DictIndexer(0, 0, Node.Literal("y")));

        var result = evaluator.Evaluate(env, variable);

        result.Should().Be(2.ToValue());
    }

    [Test]
    public void VariableIndexersCanBeChainedTogether()
    {
        var env = new Env();
        var evaluator = new VariableEvaluator();
        var value = new DictValue(
            "x".ToValue(), new ListValue(
                0.ToValue(),
                new DictValue("a".ToValue(), "result!".ToValue())
            ),
            "y".ToValue(), 2.ToValue());
        env.GlobalScope.DefineVariable("test", value);
        var variable = Node.Variable(0, 0, "test",
            Node.DictIndexer(0, 0, Node.Literal("x")),
            Node.ListIndexer(0, 0, Node.Literal(2)),
            Node.DictIndexer(0, 0, Node.Literal("a")));

        var result = evaluator.Evaluate(env, variable);

        result.Should().Be("result!".ToValue());
    }
}