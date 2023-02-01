namespace Jelly.Parser;

public interface IParserConfig
{
    bool IsWordSeparator(char ch);

    bool IsEscapeCharacter(char ch);

    IReadOnlyDictionary<char, char> EscapeCharacterSubstitutions { get; }

    bool IsEscape8bit(char ch);
    
    bool IsEscape16bit(char ch);
    
    bool IsEscape24bit(char ch);

    bool IsVariableCharacter(char ch);

    bool IsVariableDelimiter(char ch);
    
    bool IsVariableEndDelimiter(char ch);

    bool IsScriptCharacter(char ch);

    bool IsScriptEndCharacter(char ch);

    bool IsCommandSeparator(char ch);

    bool IsCommentCharacter(char ch);

    bool IsQuote(char ch);

    bool IsNestingQuote(char ch);

    bool IsNestingEndQuote(char ch);

    bool IsSpecialCharacter(char ch) => 
        IsEscapeCharacter(ch) 
        || IsVariableCharacter(ch)
        || IsScriptCharacter(ch)
        || IsScriptEndCharacter(ch)
        || IsCommandSeparator(ch)
        || IsWordSeparator(ch)
        || IsCommentCharacter(ch)
        || IsQuote(ch)
        || IsNestingQuote(ch)
        || IsNestingEndQuote(ch);
    
    string? GetOperatorAt(string source, int position);
}