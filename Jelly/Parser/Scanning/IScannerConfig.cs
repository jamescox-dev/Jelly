namespace Jelly.Parser.Scanning;

public interface IScannerConfig
{
    IReadOnlySet<char> WordSeparators { get; }

    IReadOnlySet<char> CommandSeparators { get; }

    char CommentBegin { get; }

    char CommentEnd { get; }

    char EscapeCharacter { get; }

    IReadOnlySet<char> EscapeCharacters8bit { get; }

    IReadOnlySet<char> EscapeCharacters16bit { get; }

    IReadOnlySet<char> EscapeCharacters24bit { get; }

    IReadOnlyDictionary<char, char> EscapeCharacterSubstitutions { get; }

    char VariableMarker { get; }

    char VariableBegin { get; }

    char VariableEnd { get; }

    char ScriptBegin { get; }

    char ScriptEnd { get; }

    char ExpressionBegin { get; }
    
    char ExpressionEnd { get; }

    IReadOnlySet<char> Quotes { get; }

    char NestingQuoteBegin { get; }

    char NestingQuoteEnd { get; }

    IReadOnlyList<string> Operators { get; }
}