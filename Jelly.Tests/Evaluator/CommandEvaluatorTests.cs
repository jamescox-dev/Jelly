namespace Jelly.Evaluator.Tests;

using Jelly.Commands;
using Jelly.Parser;
using Jelly.Values;

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
    public void EachArgumentIsEvaluatedAndPassedToTheCommandAlongWithTheScope()
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
    public void ArgumentNodesArePassedUnevaluatedWhenTheCommandIsAMacro()
    {
        var evaluator = new Evaluator();
        var scope = new Scope();
        var command = new TestCommand() { IsMacro = true };
        scope.DefineCommand("greet", command);
        var args = new ListValue(Node.Literal("Vic".ToValue()), Node.Literal("Bob".ToValue()));
        var commandNode = Node.Command(Node.Literal("greet".ToValue()), args);
        var commandEvaluator = new CommandEvaluator();

        commandEvaluator.Evaluate(scope, commandNode, evaluator);

        ((IComparable<Value>?)command.ArgsPassedToInvoke).Should().Be(args);
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

    public class TestCommand : ICommand
    {
        public bool IsMacro { get; set; }
        public IScope? ScopePassedToInvoke { get; private set; }
        public ListValue? ArgsPassedToInvoke { get; private set; }

        public Value Invoke(IScope scope, ListValue args)
        {
            ScopePassedToInvoke = scope;
            ArgsPassedToInvoke = args;
            return "42".ToValue();
        }
    }
}