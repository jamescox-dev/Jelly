namespace Jelly.Evaluator.Tests;

[TestFixture]
public class AssignmentEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheCorrectVariableIsSetInTheScope()
    {
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        Evaluate(assignment);

        Environment.GlobalScope.GetVariable("name").Should().Be("Bob".ToValue());
    }

    [Test]
    public void TheResultOfTheAssignmentIsTheValueOfTheAssignment()
    {
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        var result = Evaluate(assignment)

        result.Should().Be("Bob".ToValue());
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new AssignmentEvaluator();
    }
}