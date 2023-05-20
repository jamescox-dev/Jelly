namespace Jelly.Evaluator.Test;

using Jelly;



[TestFixture]
public class AssignmentEvaluatorTests
{
    [Test]
    public void TheCorrectVariableIsSetInTheScope()
    {
        var assignmentEvaluator = new AssignmentEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        assignmentEvaluator.Evaluate(scope.Object, assignment, evaluator);

        scope.Verify(m => m.SetVariable("name", "Bob".ToValue()), Times.Once);
    }

    [Test]
    public void TheResultOfTheAssignmentIsTheValueOfTheAssigment()
    {
        var assignmentEvaluator = new AssignmentEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        var result = assignmentEvaluator.Evaluate(scope.Object, assignment, evaluator);

        result.Should().Be("Bob".ToValue());
    }
}