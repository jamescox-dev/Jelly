namespace Jelly.Parser.Tests;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;

[TestFixture]
public class ExpressionParserTests
{
    [Test]
    public void AnExpressionWithNoWordsIsParsedWhenTheSourceStartsAndEndsWithAnExpressionBeginAndEnd()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("()");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(2);
        node.Should().Be(Node.Expression());
    }

    [Test]
    public void IfAnExpressionBeginIsNotFoundNoExpressionIsParsed()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("no expression");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(0);
        node.Should().BeNull();
    }

    [Test]
    public void AnExpressionWithOneWordIsParsedWhenItIsSurroundedByExpressionBeginAndEnds()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("(hi)");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(4);
        node.Should().Be(Node.Expression(Node.Literal("hi".ToValue())));
    }

    [Test]
    public void IfAnExpressionEndCharacterIsMissingFromTheEndOfTheSourceAnErrorIsThrown()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("(open");

        parser.Invoking(p => p.Parse(scanner)).Should()
            .Throw<ParseError>().WithMessage("Unexpected end-of-file.");
    }

    [Test]
    public void AnyLeadingWordSeparatorsOrCommentEndCharactersBeforeAWordAreIgnored()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("(  \n\nhi)");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(8);
        node.Should().Be(Node.Expression(Node.Literal("hi".ToValue())));
    }

    [Test]
    public void AnyTrailingWordSeparatorsOrCommentEndCharactersAfterTheLastWordAreIgnored()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("(hi\n )");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(6);
        node.Should().Be(Node.Expression(Node.Literal("hi".ToValue())));
    }

    [Test]
    public void AnExpressionCanContainMultipleWords()
    {
        var parser = new ExpressionParser();
        var scanner = new Scanner("(E = m * c ** 2)");

        var node = parser.Parse(scanner);

        scanner.Position.Should().Be(16);
        node.Should().Be(Node.Expression(
            Node.Literal("E".ToValue()),
            Node.Literal("=".ToValue()),
            Node.Literal("m".ToValue()),
            Node.Literal("*".ToValue()),
            Node.Literal("c".ToValue()),
            Node.Literal("**".ToValue()),
            Node.Literal("2".ToValue())));
    }    
}