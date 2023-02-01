namespace Jelly.Parser.Tests;

public class TestParserConfig : IParserConfig
{
    public static readonly IParserConfig Shared = new TestParserConfig();

    public IReadOnlyDictionary<char, char> EscapeCharacterSubstitutions => new Dictionary<char, char> { {'n', '\n' } };

    private TestParserConfig() {}

    public bool IsWordSeparator(char ch) => ch == ' ';

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsVariableCharacter(char ch) => ch == '$';
    
    public bool IsVariableDelimiter(char ch) => ch == '[';

    public bool IsVariableEndDelimiter(char ch) => ch == ']';

    public bool IsScriptCharacter(char ch) => ch == '{';

    public bool IsScriptEndCharacter(char ch) => ch == '}';

    public bool IsCommandSeparator(char ch) => ch == ';';

    public bool IsCommentCharacter(char ch) => ch == '#';

    public bool IsQuote(char ch) => "\'\"".Contains(ch);

    public bool IsNestingQuote(char ch) => ch == '[';

    public bool IsNestingEndQuote(char ch) => ch == ']';

    public string? GetOperatorAt(string source, int position) =>
        position < source.Length && source[position] == '=' ? "=" : null;

    public bool IsEscape8bit(char ch) => ch == 'x';

    public bool IsEscape16bit(char ch) => ch == 'u';

    public bool IsEscape24bit(char ch) => ch == 'p';
}