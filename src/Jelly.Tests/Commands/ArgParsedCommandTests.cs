namespace Jelly.Commands.Tests;

using Jelly.Runtime;

[TestFixture]
public class ArgParsedCommandTests
{
    [Test]
    public void ACommandCanBeConfiguredWithAGivenArgParserAndCommandName()
    {
        var mockParser = new Mock<IArgParser>().Object;

        var command = new ArgParsedCommand("test", mockParser, (_) => Value.Empty);

        command.ArgParser.Should().BeSameAs(mockParser);
        command.Name.Should().Be("test");
    }

    [Test]
    public void TheCommandDelegateIsCalledWithTheParsedAndEvaluatedArgumentsAndTheResultReturned()
    {
        DictionaryValue? argsPassedToCommand = null;
        Value TestCommandFunc(DictionaryValue args)
        {
            argsPassedToCommand = args;
            return "hello, jelly".ToValue();
        }
        var env = new Env();
        var argParser = new StandardArgParser(new Arg("name"));
        var command = new ArgParsedCommand("command", argParser, TestCommandFunc);

        var result = command.Invoke(env, new ListValue(Node.Literal("jelly")));

        result.Should().Be("hello, jelly".ToValue());
        argsPassedToCommand.Should().Be(new DictionaryValue(
            "name".ToValue(), "jelly".ToValue()
        ));
    }
}