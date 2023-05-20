namespace Jelly.Evaluator.Test;

using Jelly;



[TestFixture]
public class DefineVariableEvaluatorTests
{
    [Test]
    public void TheCorrectVariableIsDefinedInTheScope()
    {
        var defineVariableEvaluator = new DefineVariableEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var assignment = Node.DefineVariable("name", Node.Literal("Bob".ToValue()));

        defineVariableEvaluator.Evaluate(scope.Object, assignment, evaluator);

        scope.Verify(m => m.DefineVariable("name", "Bob".ToValue()), Times.Once);
    }

    [Test]
    public void TheResultOfTheVariableDefinitionIsTheValueOfTheVariable()
    {
        var DefineVariableEvaluator = new DefineVariableEvaluator();
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        var assignment = Node.Assignment("name", Node.Literal("Bob".ToValue()));

        var result = DefineVariableEvaluator.Evaluate(scope.Object, assignment, evaluator);

        result.Should().Be("Bob".ToValue());
    }
}