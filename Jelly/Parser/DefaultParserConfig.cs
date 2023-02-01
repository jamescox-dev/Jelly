namespace Jelly.Parser;

public class DefaultParserConfig : IParserConfig
{
    static readonly Dictionary<char, char> EscapeCharacterSubstitutionsDictionary = new() {
        { '0', '\0' },
        { 'a', '\a' }, { 'A', '\a' },
        { 'b', '\b' }, { 'B', '\b' },
        { 'f', '\f' }, { 'F', '\f' },
        { 'n', '\n' }, { 'N', '\n' },
        { 'r', '\r' }, { 'R', '\r' },
        { 't', '\t' }, { 'T', '\t' },
        { 'v', '\v' }, { 'V', '\v' },
    };

    static readonly string[] Operators = { "<=", ">=", "!=", "==", "**", "<>", "<", ">", "=", "+", "-", "*", "/", "%", "!", "&", "|", "^" };

    public IReadOnlyDictionary<char, char> EscapeCharacterSubstitutions => EscapeCharacterSubstitutionsDictionary;

    public bool IsWordSeparator(char ch) => "\t ".Contains(ch);

    public bool IsEscapeCharacter(char ch) => ch == '\\';

    public bool IsEscape8bit(char ch) => "xX".Contains(ch);
    
    public bool IsEscape16bit(char ch) => "uU".Contains(ch);
    
    public bool IsEscape24bit(char ch) => "pP".Contains(ch);

    public bool IsVariableCharacter(char ch) => ch == '$';

    public bool IsVariableDelimiter(char ch) => ch == '[';

    public bool IsVariableEndDelimiter(char ch) => ch == ']';

    public bool IsScriptCharacter(char ch) => ch == '{';

    public bool IsScriptEndCharacter(char ch) => ch == '}';

    public bool IsCommandSeparator(char ch) => "\n;".Contains(ch);

    public bool IsCommentCharacter(char ch) => ch == '#';

    public bool IsQuote(char ch) => "'\"".Contains(ch);

    public bool IsNestingQuote(char ch) => ch == '[';

    public bool IsNestingEndQuote(char ch) => ch == ']';

    public string? GetOperatorAt(string source, int position) =>
        Operators.FirstOrDefault(op => 
            position < source.Length - op.Length 
            && source[position .. (position + op.Length)].Equals(op));
}