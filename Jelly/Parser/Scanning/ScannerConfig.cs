namespace Jelly.Parser.Scanning;

using Jelly.Ast;

public class ScannerConfig : IScannerConfig
{
    public static readonly IScannerConfig Default = new ScannerConfig();

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
    
    static readonly HashSet<char> _listItemSeparators = new() { ' ', '\t', '\n', '\r' }; 

    static readonly HashSet<char> _commandSeparators = new() { '\n', ';' }; 

    static readonly HashSet<char> _wordSeparators = new() { ' ', '\t', '\r' }; 

    static readonly HashSet<char> _quotes = new() { '\'', '"' };

    static readonly Dictionary<string, Operator> _operators = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "or", Operator.Or }, { "orelse", Operator.OrElse }, { "and", Operator.And }, { "andthen", Operator.AndThen }, { "not", Operator.Not },
        { "<", Operator.LessThan }, { "<=", Operator.LessThanOrEqual }, 
        { "=", Operator.Equal }, 
        { ">=", Operator.GreaterThanOrEqual }, { ">", Operator.GreaterThan }, 
        { "<>", Operator.NotEqual },
        { "|", Operator.BitwiseOr }, { "^", Operator.BitwiseXor }, { "&", Operator.BitwiseAnd },
        { "<<", Operator.BitshiftLeft }, { ">>", Operator.BitshiftRight },
        { "+", Operator.Add }, { "-", Operator.Subtract }, { "++", Operator.Concatinate }, 
        { "*", Operator.Multiply }, {"/", Operator.Divide }, {"//", Operator.FloorDivide }, {"%", Operator.Modulo }, {"%%", Operator.FloorModulo },
        { "~", Operator.BitwiseNot },
        { "**", Operator.Exponent },
    };

    public IReadOnlySet<char> ListItemSeparators => _listItemSeparators;

    public IReadOnlySet<char> ExpressionWordSeparators => _listItemSeparators;

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

    public IReadOnlyDictionary<string, Operator> OperatorNames => _operators;
}