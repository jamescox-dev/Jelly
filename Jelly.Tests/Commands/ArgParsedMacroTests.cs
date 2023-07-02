namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class ArgParsedMacroTests
{
    [Test]
    public void AMacroCanBeConfiguredWithAGivenArgParserAndMacroName()
    {
        var mockParser = new Mock<IArgParser>().Object;

        var macro = new ArgParsedMacro("test", mockParser, (_, _) => Value.Empty);

        macro.ArgParser.Should().BeSameAs(mockParser);
        macro.Name.Should().Be("test");
    }

    [Test]
    public void TheMacroDelegateIsCalledWithTheEnvironmentAnTheParsedAndArgumentsAndTheResultEvaluatedReturned()
    {
        IEnvironment? envPassedToMacro = null;
        DictionaryValue? argsPassedToMacro = null;
        Value TestMacroFunc(IEnvironment env, DictionaryValue args)
        {
            envPassedToMacro = env;
            argsPassedToMacro = args;
            return Node.Literal("hello, jelly");
        }
        var env = new Environment();
        var argParser = new StandardArgParser(new Arg("name"));
        var macro = new ArgParsedMacro("macro", argParser, TestMacroFunc);

        var result = macro.Invoke(env, new ListValue(Node.Literal("jelly")));

        result.Should().Be("hello, jelly".ToValue());
        envPassedToMacro.Should().Be(env);
        argsPassedToMacro.Should().Be(new DictionaryValue(
            "name".ToValue(), Node.Literal("jelly")
        ));
    }
}