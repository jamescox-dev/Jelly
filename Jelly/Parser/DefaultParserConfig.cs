namespace Jelly.Parser;

public class DefaultParserConfig : IParserConfig
{
    public bool IsWordSeparator(char ch) => char.IsWhiteSpace(ch);

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsVariableCharacter(char ch) => ch == '$';

    public bool IsScriptCharacter(char ch) => ch == '{';

    public bool IsScriptEndCharacter(char ch) => ch == '}';

    public bool IsCommandSeparator(char ch) => "\n;".Contains(ch);
}