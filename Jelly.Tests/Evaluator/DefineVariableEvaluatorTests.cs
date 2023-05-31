namespace Jelly.Evaluator.Tests;

[TestFixture]
public class DefineVariableEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheCorrectVariableIsDefinedInTheScope()
    {
        var assignment = Node.DefineVariable("name", Node.Literal("Bob".ToValue()));

        Evaluate(assignment);

        Environment.GlobalScope.Invoking(s => s.GetVariable("Bob")).Should().NotThrow();
    }

    [Test]
    public void TheResultOfTheVariableDefinitionIsTheValueOfTheVariable()
    {
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        var result = Evaluate(assignment);

        result.Should().Be("Bob".ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new DefineVariableEvaluator();
    }
}