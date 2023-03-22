namespace Jelly.Library.Tests;

using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Values;

[TestFixture]
public class CoreLibraryTests
{
    CoreLibrary _lib = null!;
    Scope _scope = null!;

    [SetUp]
    public void Setup()
    {
        _lib = new CoreLibrary();
        _scope = new Scope();

        _lib.LoadIntoScope(_scope);
    }

    [TestFixture]
    public class WhenLoadedIntoAScope : CoreLibraryTests
    {
        [Test]
        public void TheScopeHasTheCorrectCommandsDefined()
        {
            _scope.Invoking(s => s.GetCommand("if")).Should().NotThrow();
            _scope.Invoking(s => s.GetCommand("var")).Should().NotThrow();
            _scope.Invoking(s => s.GetCommand("while")).Should().NotThrow();
        }
    }

    #region if

    [TestFixture]
    public class IfTests : CoreLibraryTests
    {
        [Test]
        public void WhenCalledWithoutArgumentsAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue();
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void WhenCalledWithOnlyOneArgumentAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(Node.Literal(true));
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'then_body'.");
        }

        [Test]
        public void WhenCalledWithOnlyTwoArgumentTheCorrectIfNodeIsReturned()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(true), 
                Node.Command(Node.Literal("print"), new ListValue(Node.Literal("jello, world"))));
            
            var result = ifCmd.Invoke(_scope, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(true), 
                    Node.Command(Node.Literal("print"), new ListValue(Node.Literal("jello, world")))));
        }

        [Test]
        public void AElifCanFollowAThenBody()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("ElIf"),
                Node.Literal(false),
                Node.Literal(2),
                Node.Literal("elif"),
                Node.Literal(true),
                Node.Literal(3));
            
            var result = ifCmd.Invoke(_scope, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(false),
                    Node.Literal(1),
                    Node.If(
                        Node.Literal(false),
                        Node.Literal(2),
                        Node.If(
                            Node.Literal(true),
                            Node.Literal(3)))));
        }

        [Test]
        public void AnElseCanFollowAThenBody()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("Else"),
                Node.Literal(0));
            
            var result = ifCmd.Invoke(_scope, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(false),
                    Node.Literal(1),
                    Node.Literal(0)));
        }

        [TestCase("else", false)]
        [TestCase("elif", false)]
        [TestCase("boo", true)]
        public void WhenTheWordFollowingAThenBodyIsNotElifAnErrorIsThrown(string word, bool shouldThrow)
        {
            var ifCmd = _scope.GetCommand("if");
            var args = word == "else" 
                ? new ListValue(
                    Node.Literal(false), 
                    Node.Literal(1),
                    Node.Literal(word),
                    Node.Literal(0)) 
                : new ListValue(
                    Node.Literal(false), 
                    Node.Literal(1),
                    Node.Literal(word),
                    Node.Literal(0),
                    Node.Literal(0));
            
            if (shouldThrow)
            {
                ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                    .Throw<ArgError>().WithMessage("Expected 'elif', or 'else' keyword.");
            }
            else
            {
                ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                    .NotThrow<ArgError>();
            }
        }
        
        [Test]
        public void WhenThereAreArgumentsAfterTheElseBodyAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("else"),
                Node.Literal(0),
                Node.Literal("Something"));
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected arguments after 'else_body'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAnElseBodyAfterTheElseKeywordAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("else"));
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'else_body'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAnConditionAfterTheElIfKeywordAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("elif"));
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAThenBodyAfterTheElIfKeywordAndCondtionAnErrorIsThrown()
        {
            var ifCmd = _scope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false), 
                Node.Literal(1),
                Node.Literal("elif"),
                Node.Literal(true));
            
            ifCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'then_body'.");
        }
    } 

    #endregion

    #region var

    [TestFixture]
    public class VarTests : CoreLibraryTests
    {
        [Test]
        public void TheVarCommandThrowsAnErrorWhenNoArgumentsArePassed()
        {
            var varCmd = _scope.GetCommand("var");
            var args = new ListValue();
            
            varCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'varname'.");
        }

        [Test]
        public void TheVarCommandThrowsAnErrorWhenTheSecondArgumentIsNotTheEqualsKeyword()
        {
            var varCmd = _scope.GetCommand("var");
            var args = new ListValue(Node.Variable("pi"), Node.Literal("for all!"));

            varCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected keyword '=', but found 'for all!'.");
        }

        [Test]
        public void TheVarCommandThrowsAnErrorWhenMoreThanThreeArgumentsAreGiven()
        {
            var varCmd = _scope.GetCommand("var");
            var args = new ListValue(
                Node.Variable("pi"), 
                Node.Literal("="), 
                Node.Literal("3.14159"), 
                Node.Literal("What am I doing here?"));

            varCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected value 'What am I doing here?'.");
        }

        [Test]
        public void ADefineVariableIsReturnedWithTheCorrectDetails()
        {
            var varCmd = _scope.GetCommand("var");
            var testScope = new Mock<IScope>();
            var args = new ListValue(
                Node.Literal("pi"), 
                Node.Literal("="), 
                Node.Literal("3.14159"));

            var result = varCmd.Invoke(testScope.Object, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal("3.14159")));
        }

        [Test]
        public void WhenNoValueIsSpecifiedAnEmptyValueIsUsed()
        {
            var varCmd = _scope.GetCommand("var");
            var testScope = new Mock<IScope>();
            var args = new ListValue(
                Node.Literal("pi".ToValue()), 
                Node.Literal("=".ToValue()));

            var result = varCmd.Invoke(testScope.Object, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal(Value.Empty)));
        }

        [Test]
        public void TheVarCommandDefinesAVariableWithTheEmptyValueWhenOneArgumentsIsPassed()
        {
            var varCmd = _scope.GetCommand("var");
            var testScope = new Mock<IScope>();
            var args = new ListValue(
                Node.Literal("pi".ToValue()));

            var result = varCmd.Invoke(testScope.Object, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal(Value.Empty)));
        }

        [Test]
        public void WhenTheFirstArgumentIsAVariableNodeItsNotEvaluatedAndItsNameIsUsed()
        {
            var varCmd = _scope.GetCommand("var");
            var testScope = new Mock<IScope>();
            var args = new ListValue(
                Node.Variable("pi"), 
                Node.Literal("=".ToValue()), 
                Node.Literal("3.14159".ToValue()));

            var result = varCmd.Invoke(testScope.Object, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal("3.14159")));
        }
    }

    #endregion

    #region while

    // TODO:  Change while macro to return new while node.
    [TestFixture]
    public class WhileCommand : CoreLibraryTests
    {
        [Test]
        public void TheWhileCommandThrowsAnErrorWhenNoArgumentsArePassed()
        {
            var whileCmd = _scope.GetCommand("while");
            var args = new ListValue();
            
            whileCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void TheWhileCommandThrowsAnErrorWhenOnlyOneArgumentsArePassed()
        {
            var whileCmd = _scope.GetCommand("while");
            var args = new ListValue(Node.Literal("0".ToValue()));
            
            whileCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'body'.");
        }

        [Test]
        public void TheWhileCommandThrowsAnErrorWhenMoreThanTwoArgumentsArePassed()
        {
            var whileCmd = _scope.GetCommand("while");
            var args = new ListValue(
                Node.Literal("0".ToValue()), 
                Node.Script(
                    Node.Command(Node.Literal("print".ToValue()), 
                    new ListValue())),
                Node.Literal("EXTRA!".ToValue()));
            
            whileCmd.Invoking(c => c.Invoke(_scope, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected value 'EXTRA!'.");
        }

        [Test]
        public void TheWhileCommandEvaluatesTheBodyWhileEvaluatingTheConditionIsNotZeroTheResultIsTheResultOfTheLastCommandEvaluatedInTheBody()
        {
            var condCmd = new CounterCommand { Count = 4, Increment = -1};
            var bodyCmd = new CounterCommand();
            _scope.DefineCommand("cond", condCmd);
            _scope.DefineCommand("body", bodyCmd);
            var whileCmd = _scope.GetCommand("while");
            var args = new ListValue(
                Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())), 
                Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
            
            var result = whileCmd.Invoke(_scope, args);

            bodyCmd.CallCount.Should().Be(3);
            result.Should().Be("3".ToValue());
        }

        [Test]
        public void TheWhileCommandReturnsAnEmptyValueIfTheBodyNeverRuns()
        {
    
            var condCmd = new CounterCommand { Count = 1, Increment = -1};
            var bodyCmd = new CounterCommand();
            _scope.DefineCommand("cond", condCmd);
            _scope.DefineCommand("body", bodyCmd);
            var whileCmd = _scope.GetCommand("while");
            var args = new ListValue(
                Node.Script(Node.Command(Node.Literal("cond".ToValue()), new ListValue())), 
                Node.Script(Node.Command(Node.Literal("body".ToValue()), new ListValue())));
            
            var result = whileCmd.Invoke(_scope, args);

            bodyCmd.CallCount.Should().Be(0);
            result.Should().Be(Value.Empty);
        }
    }
    #endregion
}

