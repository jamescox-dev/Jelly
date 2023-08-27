namespace Jelly.Evaluator.Tests;

[TestFixture]
public class DefineCommandEvaluatorTests : EvaluatorTestsBase
{
    [Test]
    public void TheNameOfTheCommandIsEvaluatedAndACommandWithThatNameIsDefinedInTheCurrentScope()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());

        Evaluate(defNode);

        Environment.GlobalScope.Invoking(s => s.GetCommand("test")).Should().NotThrow<NameError>();
    }

    [Test]
    public void TheCommandDefinedInTheScopeIsAUserCommandWithItBodySetCorrectly()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());
        Evaluate(defNode);

        var definedCommand = Environment.GlobalScope.GetCommand("test");

        definedCommand.Should().BeOfType<UserCommand>();
        var definedUserCommand = (UserCommand)definedCommand;
        definedUserCommand.Body.Should().Be(Node.Literal("hi"));
    }

    [Test]
    public void TheCommandDefinedInTheScopeIsHasTheCorrectRequiredArgumentNames()
    {
        var defNode = Node.DefineCommand(
            Node.Literal("test"),
            Node.Literal("body"),
            new ListValue(
                Node.Literal("a"),
                Node.Literal("b")
            ), new ListValue());
        Evaluate(defNode);

        var definedCommnad = (UserCommand)Environment.GlobalScope.GetCommand("test");

        definedCommnad.RequiredArgNames.Should().Equal("a", "b");
    }

    [Test]
    public void TheCommandDefinedInTheScopeIsHasTheCorrectOptionalArgumentNamesAndDefaultValues()
    {
        var defNode = Node.DefineCommand(
            Node.Literal("test"),
            Node.Literal("body"),
            new ListValue(
                Node.Literal("a"),
                Node.Literal("b"),
                Node.Literal("c")
            ), new ListValue(
                Node.Literal("1")
            ));
        Evaluate(defNode);

        var definedCommnad = (UserCommand)Environment.GlobalScope.GetCommand("test");

        definedCommnad.RequiredArgNames.Should().Equal("a", "b");
        definedCommnad.OptionalArgNames.Should().Equal("c");
        definedCommnad.OptionalArgDefaultValues.Should().Equal("1".ToValue());
    }

    [Test]
    public void IfTheCommandHasDuplicateArgumentNamesAnErrorIsThrown()
    {
        var defNode = Node.DefineCommand(
            Node.Literal("test"),
            Node.Literal("body"),
            new ListValue(
                Node.Literal("a"),
                Node.Literal("A")
            ),
            new ListValue());

        this.Invoking(e => e.Evaluate(defNode)).Should()
            .Throw<ArgError>().WithMessage("Argument with name 'A' already defined.");
    }

    [Test]
    public void IfTheCommandHasDuplicateRestArgumentNameAnErrorIsThrown()
    {
        var defNode = Node.DefineCommand(
            Node.Literal("test"),
            Node.Literal("body"),
            new ListValue(
                Node.Literal("a"),
                Node.Literal("b")
            ),
            new ListValue(Node.Literal(1)),
            Node.Literal("B"));

        this.Invoking(e => e.Evaluate(defNode)).Should()
            .Throw<ArgError>().WithMessage("Argument with name 'B' already defined.");
    }

    [TestCase(null)]
    [TestCase("and_the_rest")]
    public void TheCommandDefinedInTheScopeIsHasTheCorrectRestArgumentName(string? restArgumentName)
    {
        var defNode = Node.DefineCommand(
            Node.Literal("test"),
            Node.Literal("body"),
            new ListValue(), new ListValue(),
            restArgumentName is null ? null : Node.Literal("and_the_rest"));
        Evaluate(defNode);

        var definedCommnad = (UserCommand)Environment.GlobalScope.GetCommand("test");

        definedCommnad.RestArgName.Should().Be(restArgumentName);
    }

    [Test]
    public void TheResultIsAnEmptyValue()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());

        var result = Evaluate(defNode);

        result.Should().Be(Value.Empty);
    }

    protected override IEvaluator BuildEvaluatorUnderTest()
    {
        return new DefineCommandEvaluator();
    }
}