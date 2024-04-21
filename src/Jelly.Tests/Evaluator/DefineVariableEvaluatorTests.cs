namespace Jelly.Tests.Evaluator;

[TestFixture]
public class DefineVariableEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheCorrectVariableIsDefinedInTheScopeWithTheCorrectValue()
    {
        var assignment = Node.DefineVariable("name", Node.Literal("Bob".ToValue()));

        Evaluate(assignment);

        Environment.GlobalScope.GetVariable("name").Should().Be("Bob".ToValue());
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