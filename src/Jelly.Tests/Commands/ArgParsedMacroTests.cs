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
        IEnv? envPassedToMacro = null;
        DictValue? argsPassedToMacro = null;
        Value TestMacroFunc(IEnv env, DictValue args)
        {
            envPassedToMacro = env;
            argsPassedToMacro = args;
            return Node.Literal("hello, jelly");
        }
        var env = new Env();
        var argParser = new StandardArgParser(new Arg("name"));
        var macro = new ArgParsedMacro("macro", argParser, TestMacroFunc);

        var result = macro.Invoke(env, new ListValue(Node.Literal("jelly")));

        result.Should().Be("hello, jelly".ToValue());
        envPassedToMacro.Should().Be(env);
        argsPassedToMacro.Should().Be(new DictValue(
            "name".ToValue(), Node.Literal("jelly")
        ));
    }
}