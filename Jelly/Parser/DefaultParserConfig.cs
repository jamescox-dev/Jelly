namespace Jelly.Parser;

public class DefaultParserConfig : IParserConfig
{
    static readonly string[] Operators = { "<=", "=" };

    public bool IsWordSeparator(char ch) => "\t ".Contains(ch);

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsVariableCharacter(char ch) => ch == '$';

    public bool IsVariableDelimiter(char ch) => ch == '[';

    public bool IsVariableEndDelimiter(char ch) => ch == ']';

    public bool IsScriptCharacter(char ch) => ch == '{';

    public bool IsScriptEndCharacter(char ch) => ch == '}';

    public bool IsCommandSeparator(char ch) => "\n;".Contains(ch);

    public bool IsCommentCharacter(char ch) => ch == '#';

    public bool IsQuote(char ch) => "'\"".Contains(ch);

    public string? GetOperatorAt(string source, int position) =>
        Operators.FirstOrDefault(op => 
            position < source.Length - op.Length 
            && source[position .. (position + op.Length)].Equals(op));
}