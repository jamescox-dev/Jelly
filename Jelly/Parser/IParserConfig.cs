namespace Jelly.Parser;

public interface IParserConfig
{
    bool IsWordSeparator(char ch);

    bool IsEscapeCharacter(char ch);

    bool IsVariableCharacter(char ch);

    bool IsScriptCharacter(char ch);

    bool IsScriptEndCharacter(char ch);

    bool IsCommandSeparator(char ch);

    bool IsCommentCharacter(char ch);

    bool IsSpecialCharacter(char ch) => 
        IsEscapeCharacter(ch) 
        || IsVariableCharacter(ch)
        || IsScriptCharacter(ch)
        || IsScriptEndCharacter(ch)
        || IsCommandSeparator(ch)
        || IsWordSeparator(ch)
        || IsCommentCharacter(ch);
}