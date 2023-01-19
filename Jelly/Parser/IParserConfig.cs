namespace Jelly.Parser;

public interface IParserConfig
{
    bool IsWordSeparator(char ch);

    bool IsEscapeCharacter(char ch);
}