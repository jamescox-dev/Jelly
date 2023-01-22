namespace Jelly.Parser.Tests;

public class TestParserConfig : IParserConfig
{
    public static readonly IParserConfig Shared = new TestParserConfig();

    private TestParserConfig() {}

    public bool IsWordSeparator(char ch) => ch == ' ';

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsVariableCharacter(char ch) => ch == '$';
}