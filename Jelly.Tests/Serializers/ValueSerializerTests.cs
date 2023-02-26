namespace Jelly.Serializers.Tests;

using Jelly.Evaluator;
using Jelly.Parser;
using Jelly.Parser.Scanning;
using Jelly.Values;

public class ValueSerializerTests
{
    [Test]
    public void AnEmptyValueIsReturnedQuoted()
    {
        var escapedValue = ValueSerializer.Escape(string.Empty);

        escapedValue.Should().Be("''");
    }

    [TestCase("a")]
    [TestCase("a b")]
    [TestCase("a\nb")]
    [TestCase(@"a\b")]
    [TestCase(@"a;b")]
    [TestCase(@"a$b")]
    [TestCase(@"a{b")]
    [TestCase(@"a(b")]
    [TestCase(@"a)b")]
    [TestCase(@"a}b")]
    [TestCase(@"a>=b")]
    [TestCase(@"[]")]
    [TestCase(@"[\[]")]
    [TestCase(@"\[[]")]
    [TestCase(@"[\]]")]
    [TestCase(@"[]\]")]
    [TestCase(@"[[]")]
    [TestCase(@"[[]]]")]
    public void ValuesCanBeEscapedSoThatTheirValueCanBeReiterpretedByWordParserAndEvaluateBackToTheSameValue(string stringValue)
    {
        var parser = new WordParser();
        var evaluator = new Evaluator();

        var escapedValue = ValueSerializer.Escape(stringValue);
        
        evaluator.Evaluate(null!, parser.Parse(new Scanner(escapedValue))!, evaluator).ToString().Should().Be(stringValue, $"escapedValue = {escapedValue}");
    }

    [TestCase("a$b", @"a\$b", true)]
    [TestCase(@"a\b", @"a\\b", true)]
    [TestCase("a}b", @"a\}b", true)]
    [TestCase("a{b}", @"a\{b\}", false)]
    [TestCase("a b", @"a\ b", false)]
    public void ValuesShouldBeEsscapedWithBackSlashIfTheyOnlyContainOneNonWhitespaceSpecialCharacter(string stringValue, string expectedEscape, bool shouldMatch)
    {
        var escapedValue = ValueSerializer.Escape(stringValue);
        
        if (shouldMatch)
        {
            escapedValue.Should().Be(expectedEscape);
        }
        else
        {
            escapedValue.Should().NotBe(expectedEscape);
        }
    }

    [TestCase("Homer")]
    [TestCase("Marge")]
    [TestCase("Bart")]
    [TestCase("Lisa")]
    [TestCase("Maggie")]
    public void ValuesShouldNotBeEscapedIfTheyContainNoSpecialCharacters(string stringValue)
    {
        var escapedValue = ValueSerializer.Escape(stringValue);

        escapedValue.Should().Be(stringValue);
    }

    [TestCase("jello, world", "'jello, world'")]
    [TestCase("jello, 'world'", "\"jello, 'world'\"")]
    public void CharactersShouldNotBeEscapedIfTheyContainNoSpecialMeaningInAQuotedWordCharacters(string stringValue, string expectedEscape)
    {
        var escapedValue = ValueSerializer.Escape(stringValue);

        escapedValue.Should().Be(expectedEscape);
    }
}