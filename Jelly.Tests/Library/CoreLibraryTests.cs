namespace Jelly.Library.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

public class CoreLibraryTests
{
    [Test]
    public void WhenLoadedTheScopeHasTheCorrectCommandsDefined()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        
        lib.LoadIntoScope(scope);

        scope.Invoking(s =>s.GetCommand("var")).Should().NotThrow();
        scope.Invoking(s =>s.GetCommand("while")).Should().NotThrow();
    }

    #region var

    [Test]
    public void TheVarCommandThrowsAnErrorWhenNoArgumentsArePassed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var args = new ListValue();
        
        varCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Expected 'varname'.");
    }

    [Test]
    public void TheVarCommandThrowsAnErrorWhenTheSecondArgumentIsNotTheEqualsKeyword()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var args = new ListValue(Node.Variable("pi"), Node.Literal("for all!".ToValue()));

        varCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Expected keyword '=', but found 'for all!'.");
    }

    [Test]
    public void TheVarCommandThrowsAnErrorWhenMoreThanThreeArgumentsAreGiven()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var args = new ListValue(
            Node.Variable("pi"), 
            Node.Literal("=".ToValue()), 
            Node.Literal("3.14159".ToValue()), 
            Node.Literal("What am I doing here?".ToValue()));

        varCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Unexpected value 'What am I doing here?'.");
    }

    [Test]
    public void TheCorrectVariableIsDefinedInTheScopeAnTheNewValueOfTheVariableIsReturned()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var testScope = new Mock<IScope>();
        var args = new ListValue(
            Node.Literal("pi".ToValue()), 
            Node.Literal("=".ToValue()), 
            Node.Literal("3.14159".ToValue()));

        var result = varCmd.Invoke(testScope.Object, args);

        testScope.Verify(m => m.DefineVariable("pi", "3.14159".ToValue()), Times.Once);
        result.Should().Be("3.14159".ToValue());
    }

    [Test]
    public void WhenNoValueIsSpecifiedAnEmptyValueIsUsed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var testScope = new Mock<IScope>();
        var args = new ListValue(
            Node.Literal("pi".ToValue()), 
            Node.Literal("=".ToValue()));

        var result = varCmd.Invoke(testScope.Object, args);

        testScope.Verify(m => m.DefineVariable("pi", Value.Empty), Times.Once);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void TheVarCommandDefinesAVariableWithTheEmptyValueWhenOneArgumentsIsPassed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var testScope = new Mock<IScope>();
        var args = new ListValue(
            Node.Literal("pi".ToValue()));

        var result = varCmd.Invoke(testScope.Object, args);

        testScope.Verify(m => m.DefineVariable("pi", Value.Empty), Times.Once);
        result.Should().Be(Value.Empty);
    }

    [Test]
    public void WhenTheFirstArgumentIsAVariableNodeItsNotEvaluatedAndItsNameIsUsed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var varCmd = scope.GetCommand("var");
        var testScope = new Mock<IScope>();
        var args = new ListValue(
            Node.Variable("pi"), 
            Node.Literal("=".ToValue()), 
            Node.Literal("3.14159".ToValue()));

        var result = varCmd.Invoke(testScope.Object, args);

        testScope.Verify(m => m.DefineVariable("pi", "3.14159".ToValue()), Times.Once);
        result.Should().Be("3.14159".ToValue());
    }

    #endregion

    #region while

    [Test]
    public void TheWhileCommandThrowsAnErrorWhenNoArgumentsArePassed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var whileCmd = scope.GetCommand("while");
        var args = new ListValue();
        
        whileCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Expected 'condition'.");
    }

    [Test]
    public void TheWhileCommandThrowsAnErrorWhenOnlyOneArgumentsArePassed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var whileCmd = scope.GetCommand("while");
        var args = new ListValue(Node.Literal("0".ToValue()));
        
        whileCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Expected 'body'.");
    }

    [Test]
    public void TheWhileCommandThrowsAnErrorWhenMoreThanTwoArgumentsArePassed()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var whileCmd = scope.GetCommand("while");
        var args = new ListValue(
            Node.Literal("0".ToValue()), 
            Node.Script(
                Node.Command(Node.Literal("print".ToValue()), 
                new ListValue())),
            Node.Literal("EXTRA!".ToValue()));
        
        whileCmd.Invoking(c => c.Invoke(scope, args)).Should()
            .Throw<ArgError>().WithMessage("Unexpected value 'EXTRA!'.");
    }

    [Test]
    public void TheWhileCommandEvaluatesTheBodyWhileEvaluatingTheConditionIsNotZeroTheResultIsTheResultOfTheLastCommandEvaluatedInTheBody()
    {
        var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var condCmd = new CounterCommand { Count = 4, Increment = -1};
        var bodyCmd = new CounterCommand();
        scope.DefineCommand("cond", condCmd);
        scope.DefineCommand("body", bodyCmd);
        var whileCmd = scope.GetCommand("while");
        var args = new ListValue(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())), 
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
        
        var result = whileCmd.Invoke(scope, args);

        bodyCmd.CallCount.Should().Be(3);
        result.Should().Be("3".ToValue());
    }

    [Test]
    public void TheWhileCommandReturnsAnEmptyValueIfTheBodyNeverRuns()
    {
                var lib = new CoreLibrary();
        var scope = new Scope();
        lib.LoadIntoScope(scope);
        var condCmd = new CounterCommand { Count = 1, Increment = -1};
        var bodyCmd = new CounterCommand();
        scope.DefineCommand("cond", condCmd);
        scope.DefineCommand("body", bodyCmd);
        var whileCmd = scope.GetCommand("while");
        var args = new ListValue(
            Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())), 
            Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
        
        var result = whileCmd.Invoke(scope, args);

        bodyCmd.CallCount.Should().Be(0);
        result.Should().Be(Value.Empty);
    }

    #endregion
}

public class CounterCommand : ICommand
{
    public int CallCount { get; private set; }
    public int Count { get; set; }
    public int Increment { get; set; } = 1;

    public bool IsMacro => false;

    public Value Invoke(IScope scope, ListValue args)
    {
        ++CallCount;
        Count += Increment;
        return Count.ToString().ToValue();
    }
}