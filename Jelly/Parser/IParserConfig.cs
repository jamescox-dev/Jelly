namespace Jelly.Parser;

public interface IParserConfig
{
    bool IsWordSeparator(char ch);

    bool IsEscapeCharacter(char ch);

    bool IsVariableCharacter(char ch);

    bool IsSpecialCharacter(char ch) => 
        IsEscapeCharacter(ch) 
        || IsVariableCharacter(ch)
        || IsWordSeparator(ch);
}