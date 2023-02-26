namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

[TestFixture]
public class ListItemParserTests
{
    [Test]
    public void ASimpleWordCanBeParsed()
    {
        var parser = new ListItemParser();
        var scanner = new Scanner("jelly");
    
        var node = parser.Parse(scanner);

        node.Should().Be("jelly".ToValue());
    }

    [Test]
    public void AOperatorCanBeParsed()
    {
        var parser = new ListItemParser();
        var scanner = new Scanner("=");

        var node = parser.Parse(scanner);

        node.Should().Be("=".ToValue());
    }

    [Test]
    public void AQuotedWordCanBeParsed()
    {
        var parser = new ListItemParser();
        var scanner = new Scanner("'jelly'");

        var node = parser.Parse(scanner);

        node.Should().Be("jelly".ToValue());
    }

    [Test]
    public void ANestingWordCanBeParsed()
    {
        var parser = new ListItemParser();
        var scanner = new Scanner("[jelly]");

        var node = parser.Parse(scanner);

        node.Should().Be("jelly".ToValue());
    }
}