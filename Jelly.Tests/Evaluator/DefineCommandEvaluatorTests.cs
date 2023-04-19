namespace Jelly.Evaluator.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class DefineCommandEvaluatorTests
{
    IEvaluator _evaluator = null!;

    Evaluator _rootEvaluator = null!;
    Scope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _evaluator = new DefineCommandEvaluator();
        _rootEvaluator = new Evaluator();
        _scope = new Scope();
    }

    [Test]
    public void TheNameOfTheCommandIsEvaluatedAndACoomandWithThatNameIsDefinedInTheCurrentScope()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());
        
        _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        _scope.Invoking(s => s.GetCommand("test")).Should().NotThrow<NameError>();
    }

    [Test]
    public void TheCommandDefinedInTheScopeIsAUserCommandWithItBodySetCorrectly()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());
        _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        var definedCommnad = _scope.GetCommand("test");
        
        definedCommnad.Should().BeOfType<UserCommand>();
        var definedUserCommnad = (UserCommand)definedCommnad;
        definedUserCommnad.Body.Should().Be(Node.Literal("hi"));
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
        _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        var definedCommnad = (UserCommand)_scope.GetCommand("test");
        
        definedCommnad.RequiredArgumentNames.Should().Equal("a", "b");
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
        _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        var definedCommnad = (UserCommand)_scope.GetCommand("test");
        
        definedCommnad.RequiredArgumentNames.Should().Equal("a", "b");
        definedCommnad.OptionalArgumentNames.Should().Equal("c");
        definedCommnad.OptionalArgumentDefaultValues.Should().Equal("1".ToValue());
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
        
        _evaluator.Invoking(e => e.Evaluate(_scope, defNode, _rootEvaluator)).Should()
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
        
        _evaluator.Invoking(e => e.Evaluate(_scope, defNode, _rootEvaluator)).Should()
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
        _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        var definedCommnad = (UserCommand)_scope.GetCommand("test");
        
        definedCommnad.RestArgumentName.Should().Be(restArgumentName);
    }

    [Test]
    public void TheResultIsAnEmptyValue()
    {
        var defNode = Node.DefineCommand(Node.Literal("test"), Node.Literal("hi"), new ListValue(), new ListValue());
        
        var result = _evaluator.Evaluate(_scope, defNode, _rootEvaluator);

        result.Should().Be(Value.Empty);
    }
}