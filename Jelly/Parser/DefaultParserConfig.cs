namespace Jelly.Parser;

public class DefaultParserConfig : IParserConfig
{
    public bool IsWordSeparator(char ch) => char.IsWhiteSpace(ch);

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsVariableCharacter(char ch) => ch == '$';
}