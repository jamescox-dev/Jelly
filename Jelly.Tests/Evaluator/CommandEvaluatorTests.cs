namespace Jelly.Evaluator.Tests;

[TestFixture]
public class CommandEvaluatorTests
{
    [Test]
    public void TheNameOfTheCommandIsEvaluatedAndRetrievedFromTheScope()
    {
        var evaluator = new Evaluator();
        var scope = new Mock<IScope>();
        scope.Setup(m => m.GetCommand("greet")).Returns(new TestCommand());
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), new ListValue());
        var commandEvaluator = new CommandEvaluator();

        commandEvaluator.Evaluate(scope.Object, commandNode, evaluator);

        scope.Verify(m => m.GetCommand("greet"));
    }

    [Test]
    public void EachArgumentIsEvaluatedAndPassedToTheCommandAlongWithTheScopeWhenFlaggedAsSuch()
    {
        var evaluator = new Evaluator();
        var scope = new Scope();
        var command = new TestCommand();
        scope.DefineCommand("greet", command);
        var args = new ListValue(Node.Literal("Vic".ToValue()), Node.Literal("Bob".ToValue()));
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), args);
        var commandEvaluator = new CommandEvaluator();

        commandEvaluator.Evaluate(scope, commandNode, evaluator);

        ((IComparable<Value>?)command.ArgsPassedToInvoke).Should().Be(new ListValue("Vic".ToValue(), "Bob".ToValue()));
        command.ScopePassedToInvoke.Should().Be(scope);
    }

    [Test]
    public void ArgumentNodesArePassedUnevaluatedWhenTheCommandIsFlaggedAsSuch()
    {
        var evaluator = new Evaluator();
        var scope = new Scope();
        var command = new TestCommand() { EvaluationFlags = EvaluationFlags.RetrunValue, ReturnValue = Node.Literal(Value.Empty) };
        scope.DefineCommand("greet", command);
        var args = new ListValue(Node.Literal("Vic".ToValue()), Node.Literal("Bob".ToValue()));
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), args);
        var commandEvaluator = new CommandEvaluator();

        commandEvaluator.Evaluate(scope, commandNode, evaluator);

        ((IComparable<Value>?)command.ArgsPassedToInvoke).Should().Be(args);
    }

    [Test]
    public void TheReturnValueOfTheCommandIsEvaluatedAndReturnedWhenTheCommandIsFlaggedAsSuch()
    {
        var evaluator = new Evaluator();
        var scope = new Scope();
        var command = new TestCommand() { EvaluationFlags = EvaluationFlags.RetrunValue, ReturnValue = Node.Variable("test") };
        scope.DefineCommand("macro", command);
        scope.DefineVariable("test", "1 2 3".ToValue());
        var commandNode = Node.Command(Node.Literal("macro"), new ListValue());
        var commandEvaluator = new CommandEvaluator();

        var result = commandEvaluator.Evaluate(scope, commandNode, evaluator);

        result.Should().Be("1 2 3".ToValue());
    }

    [Test]
    public void TheResultOfEvaluatingACommandIsTheValueReturnedFromTheCommandsInvokeMethod()
    {
        var evaluator = new Evaluator();
        var scope = new Scope();
        var command = new TestCommand();
        scope.DefineCommand("greet", command);
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), new ListValue());
        var commandEvaluator = new CommandEvaluator();

        var result = commandEvaluator.Evaluate(scope, commandNode, evaluator);

        result.Should().Be("42".ToValue());
    }
}