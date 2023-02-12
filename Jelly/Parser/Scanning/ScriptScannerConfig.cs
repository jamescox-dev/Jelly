namespace Jelly.Parser.Scanning;

public class ScriptScannerConfig : IScannerConfig
{
    static readonly HashSet<char> _escapeCharacters8Bit = new() { 'x', 'X' };

    static readonly HashSet<char> _escapeCharacters16Bit = new() { 'u', 'U' };

    static readonly HashSet<char> _escapeCharacters24Bit = new() { 'p', 'P' };

    static readonly Dictionary<char, char> _escapeCharacterSubstitutions = new Dictionary<char, char>
    {
        { '0', '\0' },
        { 'a', '\a' }, { 'A', '\a' },
        { 'b', '\b' }, { 'B', '\b' },
        { 'f', '\f' }, { 'F', '\f' },
        { 'n', '\n' }, { 'N', '\n' },
        { 'r', '\r' }, { 'R', '\r' },
        { 't', '\t' }, { 'T', '\t' },
        { 'v', '\v' }, { 'V', '\v' },
    };

    static readonly HashSet<char> _commandSeparators = new() { '\n', ';' }; 

    static readonly HashSet<char> _wordSeparators = new() { ' ', '\t', '\r' }; 

    static readonly HashSet<char> _quotes = new() { '\'', '"' };

    static readonly List<string> _operators = new()
    { 
        "<=", ">=", "!=", "==", "<>", 
        "&&", "||",
        "++", "**", 
        "<", ">", "=", 
        "+", "-", "*", "/", "%", 
        "!", "&", "|", "^" 
    };

    public IReadOnlySet<char> WordSeparators => _wordSeparators;

    public IReadOnlySet<char> CommandSeparators => _commandSeparators;

    public char CommentBegin => '#';

    public char CommentEnd => '\n';

    public char EscapeCharacter => '\\';

    public IReadOnlySet<char> EscapeCharacters8bit => _escapeCharacters8Bit;

    public IReadOnlySet<char> EscapeCharacters16bit => _escapeCharacters16Bit;

    public IReadOnlySet<char> EscapeCharacters24bit => _escapeCharacters24Bit;

    public IReadOnlyDictionary<char, char> EscapeCharacterSubstitutions => _escapeCharacterSubstitutions;

    public char VariableMarker => '$';

    public char VariableBegin => '{';

    public char VariableEnd => '}';

    public char ScriptBegin => '{';

    public char ScriptEnd => '}';

    public char ExpressionBegin => '(';

    public char ExpressionEnd => ')';

    public IReadOnlySet<char> Quotes => _quotes;

    public char NestingQuoteBegin => '[';

    public char NestingQuoteEnd => ']';

    public IReadOnlyList<string> Operators => _operators;
}