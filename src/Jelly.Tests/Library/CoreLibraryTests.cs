namespace Jelly.Library.Tests;

using Jelly.Runtime;

[TestFixture]
public class CoreLibraryTests
{
    CoreLibrary _lib = null!;
    Env _env = null!;

    [SetUp]
    public virtual void Setup()
    {
        _lib = new CoreLibrary();
        _env = new();

        _lib.LoadIntoScope(_env.GlobalScope);
    }

    [TestFixture]
    public class WhenLoadedIntoAScope : CoreLibraryTests
    {
        [Test]
        public void TheScopeHasTheCorrectCommandsDefined()
        {
            _env.GlobalScope.Invoking(s => s.GetCommand("break")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("continue")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("def")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("for")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("if")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("defs")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("vars")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("return")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("throw")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("try")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("var")).Should().NotThrow();
            _env.GlobalScope.Invoking(s => s.GetCommand("while")).Should().NotThrow();
        }
    }

    #region break

    [TestFixture]
    public class BreakTests : CoreLibraryTests
    {
        [Test]
        public void ABreakErrorIsRaised()
        {
            var breakCmd = _env.GlobalScope.GetCommand("break");
            var args = new ListValue();

            breakCmd.Invoking(c => c.Invoke(_env, args)).Should().Throw<Break>();
        }

        [Test]
        public void AnArgErrorInThrownWhenArgumentsArePassedToBreak()
        {
            var breakCmd = _env.GlobalScope.GetCommand("break");
            var args = new ListValue(Node.Literal("boo"));

            breakCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected argument 'boo'.");
        }
    }

    #endregion

    #region continue

    [TestFixture]
    public class ContinueTests : CoreLibraryTests
    {
        [Test]
        public void ARaiseNodeOfTypeContinueIsReturned()
        {
            var continueCmd = (SimpleMacro)_env.GlobalScope.GetCommand("continue");
            var args = new ListValue();

            var result = continueCmd.InvokeMacroDelegate(_env, args);

            result.Should().Be(Node.Raise(
                Node.Literal("/continue/"), Node.Literal(Value.Empty), Node.Literal(Value.Empty)
            ));
        }

        [Test]
        public void AnArgErrorInThrownWhenArgumentsArePassedToContinue()
        {
            var continueCmd = _env.GlobalScope.GetCommand("continue");
            var args = new ListValue(Node.Literal("boo"));

            continueCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected argument 'boo'.");
        }
    }

    #endregion

    #region def

    [TestFixture]
    public class DefTests : CoreLibraryTests
    {
        SimpleMacro _defCommand = null!;

        [Test]
        public void WhenCalledWithNoArgumentsAnArgErrorIsThrown()
        {
            _defCommand.Invoking(c => c.Invoke(_env, new ListValue()))
                .Should().Throw<ArgError>().WithMessage("Expected 'name'.");
        }

        [Test]
        public void WhenCalledWithOneArgumentsAnArgErrorIsThrown()
        {
            _defCommand.Invoking(c => c.Invoke(_env, new ListValue(Node.Literal("test"))))
                .Should().Throw<ArgError>().WithMessage("Expected 'body'.");
        }

        [Test]
        public void WhenCalledWithANameAndABodyACommandDefinitionNodeIsReturned()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(Node.Literal("test"), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(), new ListValue()));
        }

        [Test]
        public void WhenCalledWithANameASingleArgumentNameAndABodyACommandDefinitionNodeIsReturned()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(Node.Literal("test"), Node.Literal("name"), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(Node.Literal("name")), new ListValue()));
        }

        [Test]
        public void WhenCalledWithANameASingleVariableArgumentNameAndABodyACommandDefinitionNodeWithTheVariableAsALiteralIsReturned()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(Node.Literal("test"), Node.Variable("name"), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(Node.Literal("name")), new ListValue()));
        }

        [Test]
        public void WhenCalledWithANameMultipleArgumentNamesAndABodyACommandDefinitionNodeIsReturned()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(Node.Literal("test"), Node.Literal("a"), Node.Variable("b"), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(Node.Literal("a"), Node.Literal("b")), new ListValue()));
        }

        [Test]
        public void WhenAnArgumentNameIsFollowedByAnEqualsKeyWordTheFollowingArgumentIsConsideredItsDefaultValueCommandDefinitionNodeIsReturned()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(Node.Literal("test"), Node.Literal("name"), Node.Literal("="), Node.Literal("world"), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(Node.Literal("name")), new ListValue(Node.Literal("world"))));
        }

        [Test]
        public void WhenTheLastArgumentNameIsFollowedByAnEqualsThenTheFunctionBodyTheLastArgumentBecomesTheRestArgument()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(
                Node.Literal("test"),
                Node.Literal("name"), Node.Literal("="), Node.Literal("world"),
                Node.Literal("rest"), Node.Literal("="), Node.Literal("body")));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Literal("body"), new ListValue(Node.Literal("name")), new ListValue(Node.Literal("world")), Node.Literal("rest")));
        }

        [Test]
        public void BugThreeEqualsSignsShouldNotThrowNotThrow()
        {
            var result = _defCommand.InvokeMacroDelegate(_env, new ListValue(
                Node.Literal("test"),
                Node.Literal("="), Node.Literal("="), Node.Literal("="), Node.Script()));

            result.Should().Be(Node.DefineCommand(
                Node.Literal("test"), Node.Script(), new ListValue(Node.Literal("=")), new ListValue(Node.Literal("="))));
        }

        [Test]
        public void ARequiredArgumentCanNotFollowAOptionalArgument()
        {
            _defCommand.Invoking(c => c.Invoke(_env, new ListValue(
                Node.Literal("test"),
                Node.Literal("a"), Node.Literal("="), Node.Literal("1"), Node.Literal("b"), Node.Literal("c"), Node.Script()))).Should()
                    .Throw<ArgError>().WithMessage("Argument 'b' must have a default value.");
        }

        [Test]
        public void ARequiredArgumentCanNotBeTheLastArgumentFollowingAnOptionalArgument()
        {
            _defCommand.Invoking(c => c.Invoke(_env, new ListValue(
                Node.Literal("test"),
                Node.Literal("a"), Node.Literal("="), Node.Literal("1"), Node.Literal("b"), Node.Script()))).Should()
                    .Throw<ArgError>().WithMessage("Argument 'b' must have a default value.");
        }

        public override void Setup()
        {
            base.Setup();
            _defCommand = (SimpleMacro)_env.GlobalScope.GetCommand("def");
        }
    }

    #endregion

    #region for

    [TestFixture]
    public class ForTests : CoreLibraryTests
    {
        [Test]
        public void WhenAListArgumentIsPassedTheBodyIsEvaluatedForEachItemInTheListAndTheResultIsThatOfTheLastEvaluation()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("i"),
                Node.Literal("in"),
                Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("i"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue("a".ToValue()),
                new ListValue("b".ToValue()),
                new ListValue("c".ToValue()));
        }

        [Test]
        public void AnOptionalIndexIteratorCanBeSuppliedForLists()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("i"),
                Node.Literal("v"),
                Node.Literal("in"),
                Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("i"), Node.Variable("v"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue(1.ToValue(), "a".ToValue()),
                new ListValue(2.ToValue(), "b".ToValue()),
                new ListValue(3.ToValue(), "c".ToValue()));
        }

        [Test]
        public void TheIndexAndValueIteratorsCanBeGivenAsVariables()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Variable("iv"),
                Node.Variable("vv"),
                Node.Literal("in"),
                Node.Literal(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("iv"), Node.Variable("vv"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue(1.ToValue(), "a".ToValue()),
                new ListValue(2.ToValue(), "b".ToValue()),
                new ListValue(3.ToValue(), "c".ToValue()));
        }

        [Test]
        public void WhenADictArgumentIsPassedTheBodyIsEvaluatedForEachItemInTheDictionaryAndTheResultIsThatOfTheLastEvaluation()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("k"),
                Node.Literal("of"),
                Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("k"))))
            ));

            result.Should().Be(2.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue("a".ToValue()),
                new ListValue("b".ToValue()));
        }

        [Test]
        public void AnOptionalValueIteratorCanBeSuppliedForDicts()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("k"),
                Node.Literal("v"),
                Node.Literal("of"),
                Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("k"), Node.Variable("v"))))
            ));

            result.Should().Be(2.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue("a".ToValue(), 1.ToValue()),
                new ListValue("b".ToValue(), 2.ToValue()));
        }

        [Test]
        public void TheKeyAndValueIteratorsCanBeGivenAsVariables()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Variable("kv"),
                Node.Variable("vv"),
                Node.Literal("of"),
                Node.Literal(new DictValue("a".ToValue(), 1.ToValue(), "b".ToValue(), 2.ToValue())),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("kv"), Node.Variable("vv"))))
            ));

            result.Should().Be(2.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue("a".ToValue(), 1.ToValue()),
                new ListValue("b".ToValue(), 2.ToValue()));
        }

        [Test]
        public void WhenAStartArgumentIsPassedTheBodyIsEvaluatedForEachValueBetweenTheStartAndEndValueAndTheResultIsThatOfTheLastEvaluation()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("i"),
                Node.Literal("="),
                Node.Literal("1"),
                Node.Literal("to"),
                Node.Literal("3"),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("i"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue(1.ToValue()),
                new ListValue(2.ToValue()),
                new ListValue(3.ToValue()));
        }

        [Test]
        public void AnOptionalStepCanBeSuppliedForRanges()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Literal("i"),
                Node.Literal("="),
                Node.Literal("2"),
                Node.Literal("to"),
                Node.Literal("6"),
                Node.Literal("step"),
                Node.Literal("2"),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("i"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue(2.ToValue()),
                new ListValue(4.ToValue()),
                new ListValue(6.ToValue()));
        }

        [Test]
        public void TheIteratorCanBeGivenAsAVariable()
        {
            var forCmd = _env.GlobalScope.GetCommand("for");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = forCmd.Invoke(_env, new ListValue(
                Node.Variable("iv"),
                Node.Literal("="),
                Node.Literal("2"),
                Node.Literal("to"),
                Node.Literal("6"),
                Node.Literal("step"),
                Node.Literal("2"),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), new ListValue(Node.Variable("iv"))))
            ));

            result.Should().Be(3.ToValue());
            bodyCommand.RecordedArguments.Should().Equal(
                new ListValue(2.ToValue()),
                new ListValue(4.ToValue()),
                new ListValue(6.ToValue()));
        }
    }

    #endregion

    #region if

    [TestFixture]
    public class IfTests : CoreLibraryTests
    {
        [Test]
        public void WhenCalledWithoutArgumentsAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue();

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void WhenCalledWithOnlyOneArgumentAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(Node.Literal(true));

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'then_body'.");
        }

        [Test]
        public void WhenCalledWithOnlyTwoArgumentTheCorrectIfNodeIsReturned()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(true),
                Node.Command(Node.Literal("print"), new ListValue(Node.Literal("jello, world"))));

            var result = ifCmd.InvokeMacroDelegate(_env, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(true),
                    Node.Scope(Node.Command(Node.Literal("print"), new ListValue(Node.Literal("jello, world"))))));
        }

        [Test]
        public void AElifCanFollowAThenBody()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("ElIf"),
                Node.Literal(false),
                Node.Literal(2),
                Node.Literal("elif"),
                Node.Literal(true),
                Node.Literal(3));

            var result = ifCmd.InvokeMacroDelegate(_env, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(false),
                    Node.Scope(Node.Literal(1)),
                    Node.If(
                        Node.Literal(false),
                        Node.Scope(Node.Literal(2)),
                        Node.If(
                            Node.Literal(true),
                            Node.Scope(Node.Literal(3))))));
        }

        [Test]
        public void AnElseCanFollowAThenBody()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("Else"),
                Node.Literal(0));

            var result = ifCmd.InvokeMacroDelegate(_env, args);

            result.Should().Be(
                Node.If(
                    Node.Literal(false),
                    Node.Scope(Node.Literal(1)),
                    Node.Scope(Node.Literal(0))));
        }

        [TestCase("else", false)]
        [TestCase("elif", false)]
        [TestCase("boo", true)]
        public void WhenTheWordFollowingAThenBodyIsNotElifAnErrorIsThrown(string word, bool shouldThrow)
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
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
                ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                    .Throw<ArgError>().WithMessage("Expected 'elif', or 'else' keyword.");
            }
            else
            {
                ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                    .NotThrow<ArgError>();
            }
        }

        [Test]
        public void WhenThereAreArgumentsAfterTheElseBodyAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("else"),
                Node.Literal(0),
                Node.Literal("Something"));

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected arguments after 'else_body'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAnElseBodyAfterTheElseKeywordAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("else"));

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'else_body'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAnConditionAfterTheElIfKeywordAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("elif"));

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void IfThereIsNotArgumentForAThenBodyAfterTheElIfKeywordAndConditionAnErrorIsThrown()
        {
            var ifCmd = (SimpleMacro)_env.GlobalScope.GetCommand("if");
            var args = new ListValue(
                Node.Literal(false),
                Node.Literal(1),
                Node.Literal("elif"),
                Node.Literal(true));

            ifCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'then_body'.");
        }
    }

    #endregion

    #region defs

    [TestFixture]
    public class LsDefTests : CoreLibraryTests
    {
        [Test]
        public void ReturnsAListOfEachCommandDefinedInTheCurrentScopeAnSurroundingScope()
        {
            var lsDefCmd = _env.GlobalScope.GetCommand("defs");
            var outerScope = new Scope();
            outerScope.DefineCommand("d", new SimpleCommand((_) => Value.Empty));
            outerScope.DefineCommand("b", new SimpleCommand((_) => Value.Empty));
            var scope = new Scope(outerScope);
            scope.DefineCommand("c", new SimpleCommand((_) => Value.Empty));
            scope.DefineCommand("a", new SimpleCommand((_) => Value.Empty));
            var env = new Env(scope);

            var result = lsDefCmd.Invoke(env, new ListValue());

            result.Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue()));
        }

        [Test]
        public void ReturnsAListOfEachCommandDefinedInTheCurrentScopeOnlyWhenSpecified()
        {
            var lsDefCmd = _env.GlobalScope.GetCommand("defs");
            var outerScope = new Scope();
            outerScope.DefineCommand("d", new SimpleCommand((_) => Value.Empty));
            outerScope.DefineCommand("b", new SimpleCommand((_) => Value.Empty));
            var scope = new Scope(outerScope);
            scope.DefineCommand("c", new SimpleCommand((_) => Value.Empty));
            scope.DefineCommand("a", new SimpleCommand((_) => Value.Empty));
            var env = new Env(scope);

            var result = lsDefCmd.Invoke(env, new ListValue(Node.Literal(true)));

            result.Should().Be(new ListValue("a".ToValue(), "c".ToValue()));
        }
    }

    #endregion

    #region vars

    [TestFixture]
    public class LsVarTests : CoreLibraryTests
    {
        [Test]
        public void ReturnsAListOfEachVariableDefinedInTheCurrentScopeAnSurroundingScope()
        {
            var lsVarCmd = _env.GlobalScope.GetCommand("vars");
            var outerScope = new Scope();
            outerScope.DefineVariable("d", Value.Empty);
            outerScope.DefineVariable("b", Value.Empty);
            var scope = new Scope(outerScope);
            scope.DefineVariable("c", Value.Empty);
            scope.DefineVariable("a", Value.Empty);
            var env = new Env(scope);

            var result = lsVarCmd.Invoke(env, new ListValue());

            result.Should().Be(new ListValue("a".ToValue(), "b".ToValue(), "c".ToValue(), "d".ToValue()));
        }

        [Test]
        public void ReturnsAListOfEachVariableDefinedInTheCurrentScopeOnlyWhenSpecified()
        {
            var lsVarCmd = _env.GlobalScope.GetCommand("vars");
            var outerScope = new Scope();
            outerScope.DefineVariable("d", Value.Empty);
            outerScope.DefineVariable("b", Value.Empty);
            var scope = new Scope(outerScope);
            scope.DefineVariable("c", Value.Empty);
            scope.DefineVariable("a", Value.Empty);
            var env = new Env(scope);

            var result = lsVarCmd.Invoke(env, new ListValue(Node.Literal(true)));

            result.Should().Be(new ListValue("a".ToValue(), "c".ToValue()));
        }
    }

    #endregion

    #region throw

    [TestFixture]
    public class RaiseTests : CoreLibraryTests
    {
        [Test]
        public void WithNoArgumentsAnArgErrorIsThrow()
        {
            var raiseCmd = (SimpleMacro)_env.GlobalScope.GetCommand("throw");
            var env = new Env();
            var args = new ListValue();

            raiseCmd.Invoking(c => c.Invoke(env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'type' argument.");
        }

        [Test]
        public void WithOneArgumentsARaiseNodeIsReturnedWithTheCorrectType()
        {
            var raiseCmd = (SimpleMacro)_env.GlobalScope.GetCommand("throw");
            var env = new Env();
            var args = new ListValue(Node.Literal("/error/type"));

            var result = raiseCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.Raise(
                Node.Literal("/error/type"),
                Node.Literal(Value.Empty),
                Node.Literal(Value.Empty)));
        }

        [Test]
        public void WithTwoArgumentsARaiseNodeIsReturnedWithTheCorrectTypeAndMessage()
        {
            var raiseCmd = (SimpleMacro)_env.GlobalScope.GetCommand("throw");
            var env = new Env();
            var args = new ListValue(Node.Literal("/error/type"), Node.Literal("Test message."));

            var result = raiseCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.Raise(
                Node.Literal("/error/type"),
                Node.Literal("Test message."),
                Node.Literal(Value.Empty)));
        }

        [Test]
        public void WithThreeArgumentsARaiseNodeIsReturnedWithTheCorrectTypeMessageAndValue()
        {
            var raiseCmd = (SimpleMacro)_env.GlobalScope.GetCommand("throw");
            var env = new Env();
            var args = new ListValue(Node.Literal("/error/type"), Node.Literal("Test message."), Node.Literal("value"));

            var result = raiseCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.Raise(
                Node.Literal("/error/type"),
                Node.Literal("Test message."),
                Node.Literal("value")));
        }

        [Test]
        public void WithMoreThanThreeArgumentsAnArgErrorIsThrow()
        {
            var raiseCmd = (SimpleMacro)_env.GlobalScope.GetCommand("throw");
            var env = new Env();
            var args = new ListValue(Value.Empty, Value.Empty, Value.Empty, Node.Literal("boo"));

            raiseCmd.Invoking(c => c.Invoke(env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected argument 'boo'.");
        }
    }

    #endregion

    #region repeat

    [TestFixture]
    public class RepeatTests : CoreLibraryTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void TheBodyIsEvaluatedTheGivenNumberOfTimesAnTheValueOfTheLastEvaluationIsReturned(int times) 
        {
            var repeatCmd = _env.GlobalScope.GetCommand("repeat");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = repeatCmd.Invoke(_env, new ListValue(
                Node.Literal(times),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), ListValue.EmptyList))
            ));

            result.Should().Be(times.ToValue());
            bodyCommand.Invocations.Should().Be(times);
        }

        [TestCase(2.1)]
        [TestCase(2.5)]
        [TestCase(2.9)]
        public void TheBodyIsEvaluatedTheGivenNumberOfTimesWhenTimesIsARealNumberItIsRoundedDownToTheNearestWholeNumber(
            double times) 
        {
            var repeatCmd = _env.GlobalScope.GetCommand("repeat");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            repeatCmd.Invoke(_env, new ListValue(
                Node.Literal(times),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), ListValue.EmptyList))
            ));

            bodyCommand.Invocations.Should().Be(2);
        }

        [TestCase(0.0)]
        [TestCase(-1.0)]
        [TestCase(double.NegativeInfinity)]
        public void TheBodyIsNotEvaluatedWhenTimesIsZeroOrLessAndTheResultIsAnEmptyValue(double times) 
        {
            var repeatCmd = _env.GlobalScope.GetCommand("repeat");
            var bodyCommand = new RecordingTestCommand();
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            var result = repeatCmd.Invoke(_env, new ListValue(
                Node.Literal(times),
                Node.Script(Node.Command(Node.Literal("bodyCommand"), ListValue.EmptyList))
            ));

            result.Should().Be(Value.Empty);
            bodyCommand.Invocations.Should().Be(0);
        }

        [Test]
        public void WhenTimesIsNotSpecifiedTheBodyIsEvaluatedUntilABreakIsThrown()
        {
            var count = 0;
            var repeatCmd = _env.GlobalScope.GetCommand("repeat");
            var bodyCommand = new SimpleCommand((_) => {
                ++count;
                if (count == 100) 
                {
                    throw Error.Break();
                }
                return Value.Empty;
            });
            _env.GlobalScope.DefineCommand("bodyCommand", bodyCommand);

            repeatCmd.Invoke(_env, new ListValue(
                Node.Script(Node.Command(Node.Literal("bodyCommand"), ListValue.EmptyList))
            ));

            count.Should().Be(100);
        }
    }

    #endregion

    #region return

    [TestFixture]
    public class ReturnTests : CoreLibraryTests
    {
        [Test]
        public void WithoutArgumentsARaiseNodeWithAnEmptyValueIsReturned()
        {
            var returnCmd = (SimpleMacro)_env.GlobalScope.GetCommand("return");
            var env = new Env();
            var args = new ListValue();

            var result = returnCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(
                Node.Raise(
                    Node.Literal("/return/"),
                    Node.Literal(Value.Empty),
                    Node.Literal(Value.Empty)));
        }

        [Test]
        public void WithOneArgumentARaiseNodeIsReturnedWithTheCorrectReturnValue()
        {
            var returnCmd = (SimpleMacro)_env.GlobalScope.GetCommand("return");
            var env = new Env();
            var args = new ListValue(Node.Variable("name"));

            var result = returnCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(
                Node.Raise(
                    Node.Literal("/return/"),
                    Node.Literal(Value.Empty),
                    Node.Variable("name")));
        }

        [Test]
        public void WithMoreThanOneArgumentAnErrorIsThrown()
        {
            var returnCmd = (SimpleMacro)_env.GlobalScope.GetCommand("return");
            var env = new Env();
            var args = new ListValue(Node.Variable("name"), Node.Literal("boo"));

            returnCmd.Invoking(c => c.Invoke(env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected argument 'boo'.");
        }
    }

    #endregion

    #region try

    [TestFixture]
    public class TryTests : CoreLibraryTests
    {
        [Test]
        public void WhenNoArgumentsArePassedAnArgErrorIsThrown()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue())).Should()
                .Throw<ArgError>().WithMessage("Expected 'body' argument.");
        }

        [Test]
        public void WhenOnlyOneArgumentIsPassedAnSimpleTryNodeReturned()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");

            var result = tryCmd.InvokeMacroDelegate(_env, new ListValue(Node.Script(Node.Command(Node.Literal("print"), new ListValue()))));

            result.Should().Be(Node.Try(
                Node.Scope(Node.Script(Node.Command(Node.Literal("print"), new ListValue()))),
                null
            ));
        }

        [TestCase("finally")]
        [TestCase("Finally")]
        [TestCase("FINALLY")]
        public void WhenTheSecondToLastArgumentIsTheFinallyKeywordTheFinalArgumentIsIncludedAsTheFinallyBody(string finallyKeyword)
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            var finallyBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue(finallyKeyword.ToValue())));

            var result = tryCmd.InvokeMacroDelegate(_env, new ListValue(
                body, Node.Literal("finally".ToValue()), finallyBody));

            result.Should().Be(Node.Try(
                Node.Scope(body),
                Node.Scope(finallyBody)
            ));
        }

        [TestCase("FinNaLY")]
        [TestCase("finalLY")]
        [TestCase("FInalLY")]
        public void WhenTheLastArgumentIsTheFinallyKeywordAnErrorIsThrown(string finallyKeyword)
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            Node.Script(Node.Command(Node.Literal("print"), new ListValue(finallyKeyword.ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(
                body, Node.Literal("finally".ToValue())))).Should()
                .Throw<ArgError>("Expected 'finally_body' argument.");
        }

        [TestCase("catch")]
        [TestCase("CaTcH")]
        public void WhenTheThirdArgumentBeginsWithTheCatchKeywordButIsNotFollowedByAnyOtherArgumentsAnErrorIsThrows(string exceptKeyword)
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(body, Node.Literal(exceptKeyword)))).Should()
                .Throw<ArgError>().WithMessage("Expected 'error_details' argument.");
        }

        [TestCase("CATCH")]
        [TestCase("catcH")]
        public void WhenTheThirdArgumentBeginsWithTheExceptKeywordAndIsFollowedByErrorDetailsButIsNotFollowedByAnyOtherArgumentsAnErrorIsThrows(string exceptKeyword)
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(body, Node.Literal(exceptKeyword), Node.Literal("/error/")))).Should()
                .Throw<ArgError>().WithMessage("Expected 'except_body' argument.");
        }

        [TestCase("CAtch")]
        [TestCase("caTCH")]
        public void WhenTheThirdArgumentBeginsWithTheExceptKeywordAndIsFollowedByErrorDetailsExceptBodyTheCorrectTryNodeIsReturned(string exceptKeyword)
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            var excepetBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue()));

            var result = tryCmd.InvokeMacroDelegate(_env, new ListValue(
                body, Node.Literal(exceptKeyword), Node.Literal("/error/"), excepetBody));

            result.Should().Be(Node.Try(
                Node.Scope(body),
                null,
                (Node.Literal("/error/"), Node.Scope(excepetBody))
            ));
        }

        [Test]
        public void MultipleExceptClausesCanBeParsed()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            var excepetBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue()));

            var result = tryCmd.InvokeMacroDelegate(_env, new ListValue(
                body,
                Node.Literal("catch"), Node.Literal("/error/arg"), excepetBody,
                Node.Literal("catch"), Node.Literal("/error/type"), excepetBody));

            result.Should().Be(Node.Try(
                Node.Scope(body),
                null,
                (Node.Literal("/error/arg"), Node.Scope(excepetBody)),
                (Node.Literal("/error/type"), Node.Scope(excepetBody))
            ));
        }

        [Test]
        public void ExceptClausesCanBeFollowedByAFinallyClause()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            var excepetBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue()));
            var finallyBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue("finally".ToValue())));

            var result = tryCmd.InvokeMacroDelegate(_env, new ListValue(
                body,
                Node.Literal("catch"), Node.Literal("/error/arg"), excepetBody,
                Node.Literal("finally"), finallyBody));

            result.Should().Be(Node.Try(
                Node.Scope(body),
                Node.Scope(finallyBody),
                (Node.Literal("/error/arg"), Node.Scope(excepetBody))
            ));
        }

        [Test]
        public void AnErrorIsThrownIfAnotherValueOtherThanTheExceptOrFinallyKeywordsAreFoundWhenOneOfTheKeywordWouldHaveBeenExpected()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(body, Node.Literal("nonsense")))).Should()
                .Throw<ArgError>().WithMessage("Unexpected 'nonsense' argument.");
        }

        [Test]
        public void ACatchClauseCanNotComeAfterAFinallyClause()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            var excepetBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue()));
            var finallyBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue("finally".ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(
                body,
                Node.Literal("finally"), finallyBody,
                Node.Literal("catch"), Node.Literal("/error/arg"), excepetBody))).Should()
                    .Throw<ArgError>("Unexpected 'catch' argument after 'finally'.");
        }

        [Test]
        public void ATryCanOnlyHaveOneFinallyClause()
        {
            var tryCmd = (SimpleMacro)_env.GlobalScope.GetCommand("try");
            var body = Node.Script(Node.Command(Node.Literal("print"), new ListValue("test".ToValue())));
            Node.Script(Node.Command(Node.Literal("print"), new ListValue()));
            var finallyBody = Node.Script(Node.Command(Node.Literal("print"), new ListValue("finally".ToValue())));

            tryCmd.Invoking(c => c.Invoke(_env, new ListValue(
                body,
                Node.Literal("finally"), finallyBody,
                Node.Literal("finally"), finallyBody))).Should()
                    .Throw<ArgError>("Unexpected duplicate 'finally' argument.");
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
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var args = new ListValue();

            varCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'varname'.");
        }

        [Test]
        public void TheVarCommandThrowsAnErrorWhenTheSecondArgumentIsNotTheEqualsKeyword()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var args = new ListValue(Node.Variable("pi"), Node.Literal("for all!"));

            varCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected keyword '=', but found 'for all!'.");
        }

        [Test]
        public void TheVarCommandThrowsAnErrorWhenMoreThanThreeArgumentsAreGiven()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var args = new ListValue(
                Node.Variable("pi"),
                Node.Literal("="),
                Node.Literal("3.14159"),
                Node.Literal("What am I doing here?"));

            varCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected value 'What am I doing here?'.");
        }

        [Test]
        public void ADefineVariableIsReturnedWithTheCorrectDetails()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var env = new Env();
            var args = new ListValue(
                Node.Literal("pi"),
                Node.Literal("="),
                Node.Literal("3.14159"));

            var result = varCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal("3.14159")));
        }

        [Test]
        public void WhenNoValueIsSpecifiedAnEmptyValueIsUsed()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var env = new Env();
            var args = new ListValue(
                Node.Literal("pi".ToValue()),
                Node.Literal("=".ToValue()));

            var result = varCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal(Value.Empty)));
        }

        [Test]
        public void TheVarCommandDefinesAVariableWithTheEmptyValueWhenOneArgumentsIsPassed()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var env = new Env();
            var args = new ListValue(
                Node.Literal("pi".ToValue()));

            var result = varCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal(Value.Empty)));
        }

        [Test]
        public void WhenTheFirstArgumentIsAVariableNodeItsNotEvaluatedAndItsNameIsUsed()
        {
            var varCmd = (SimpleMacro)_env.GlobalScope.GetCommand("var");
            var env = new Env();
            var args = new ListValue(
                Node.Variable("pi"),
                Node.Literal("=".ToValue()),
                Node.Literal("3.14159".ToValue()));

            var result = varCmd.InvokeMacroDelegate(env, args);

            result.Should().Be(Node.DefineVariable("pi", Node.Literal("3.14159")));
        }
    }

    #endregion

    #region while

    [TestFixture]
    public class WhileCommand : CoreLibraryTests
    {
        [Test]
        public void TheWhileCommandThrowsAnErrorWhenNoArgumentsArePassed()
        {
            var whileCmd = _env.GlobalScope.GetCommand("while");
            var args = new ListValue();

            whileCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'condition'.");
        }

        [Test]
        public void TheWhileCommandThrowsAnErrorWhenOnlyOneArgumentsArePassed()
        {
            var whileCmd = _env.GlobalScope.GetCommand("while");
            var args = new ListValue(Node.Literal("0".ToValue()));

            whileCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Expected 'body'.");
        }

        [Test]
        public void TheWhileCommandThrowsAnErrorWhenMoreThanTwoArgumentsArePassed()
        {
            var whileCmd = _env.GlobalScope.GetCommand("while");
            var args = new ListValue(
                Node.Literal("0".ToValue()),
                Node.Script(
                    Node.Command(Node.Literal("print".ToValue()),
                    new ListValue())),
                Node.Literal("EXTRA!".ToValue()));

            whileCmd.Invoking(c => c.Invoke(_env, args)).Should()
                .Throw<ArgError>().WithMessage("Unexpected value 'EXTRA!'.");
        }

        [Test]
        public void TheCorrectWhileNodeIsReturned()
        {
            var whileCmd = (SimpleMacro)_env.GlobalScope.GetCommand("while");
            var args = new ListValue(
                Node.Literal("0"),
                Node.Script(
                    Node.Command(Node.Literal("print"),
                    new ListValue())));

            var result = whileCmd.InvokeMacroDelegate(_env, args);

            result.Should().Be(Node.While(Node.Literal("0"),
                Node.Scope(Node.Script(
                    Node.Command(Node.Literal("print"),
                    new ListValue())))));
        }
    }

    #endregion
}

