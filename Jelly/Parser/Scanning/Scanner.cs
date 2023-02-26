namespace Jelly.Parser.Scanning;

public class Scanner
{
    public IScannerConfig Config { get; private set; }

    public string Source { get; private set; }

    public int Position { get; private set; }

    public Scanner(string source)
    {
        Config = ScannerConfig.Default;
        Source = source;
        Position = 0;
    }

    public Scanner(string source, int position)
    {
        Config = ScannerConfig.Default;
        Source = source;
        Position = position;
    }

    public void Advance(int amount = 1)
    {
        Position += amount;
    }

    public bool AdvanceIf(Func<Scanner, bool> condtion, int amount = 1)
    {
        var result = condtion(this);

        if (result)
        {
            Advance(amount);
        }

        return result;
    }

    public int AdvanceWhile(Func<Scanner, bool> condition)
    {
        var amount = 0;
        
        while (!IsEof && condition(this))
        {
            ++Position;
            ++amount;
        }

        return amount;
    }

    public char? CurrentCharacter => Position >= 0 && Position < Source.Length
        ? Source[Position]
        : null;
    
    public string Substring(int length)
    {
        var start = Math.Max(Position, 0);
        var len = Math.Min(Position + length, Source.Length) - start;

        return Source.Substring(start, len);
    }      

    public bool IsEof => Position >= Source.Length;

    public bool IsEscapeCharacter => CurrentCharacter == Config.EscapeCharacter;

    public bool IsEscapeCharacter8bit => Config.EscapeCharacters8bit.Contains(CurrentCharacter ?? '\0');

    public bool IsEscapeCharacter16bit => Config.EscapeCharacters16bit.Contains(CurrentCharacter ?? '\0');

    public bool IsEscapeCharacter24bit => Config.EscapeCharacters24bit.Contains(CurrentCharacter ?? '\0');

    public bool IsEscapeSubstitutableCharacter => Config.EscapeCharacterSubstitutions.ContainsKey(CurrentCharacter ?? '\0');

    public char? SubstitedEscapeCharacter => CurrentCharacter is not null 
        ? Config.EscapeCharacterSubstitutions.GetValueOrDefault((char)CurrentCharacter!, (char)CurrentCharacter!)
        : null;

    public bool IsCommandSeparator => Config.CommandSeparators.Contains(CurrentCharacter ?? '\0');

    public bool IsWordSeparator => Config.WordSeparators.Contains(CurrentCharacter ?? '\0');

    public bool IsCommentBegin => CurrentCharacter == Config.CommentBegin;

    public bool IsCommentEnd => CurrentCharacter == Config.CommentEnd;
    
    public bool IsNestingQuoteBegin => CurrentCharacter == Config.NestingQuoteBegin;
    
    public bool IsNestingQuoteEnd => CurrentCharacter == Config.NestingQuoteEnd;

    public bool IsQuote => Config.Quotes.Contains(CurrentCharacter ?? '\0');

    public bool IsScriptBegin => CurrentCharacter == Config.ScriptBegin;

    public bool IsScriptEnd => CurrentCharacter == Config.ScriptEnd;
    
    public bool IsExpressionBegin => CurrentCharacter == Config.ExpressionBegin;
    
    public bool IsExpressionEnd => CurrentCharacter == Config.ExpressionEnd;

    public bool IsVariableMarker => CurrentCharacter == Config.VariableMarker;
    
    public bool IsVariableBegin => CurrentCharacter == Config.VariableBegin;
    
    public bool IsVariableEnd => CurrentCharacter == Config.VariableEnd;
    
    public bool IsSpecialCharacter =>
        IsEscapeCharacter || IsVariableMarker 
        || IsCommandSeparator || IsWordSeparator
        || IsExpressionBegin || IsExpressionEnd
        || IsScriptBegin || IsScriptEnd
        || IsNestingQuoteBegin || IsNestingQuoteEnd
        || IsQuote || IsCommentBegin
        || TryGetOperator(out var _);

    public bool TryGetOperator(out string op)
    {
        foreach (var candidate in Config.Operators)
        {
            if (Substring(candidate.Length) == candidate)
            {
                op = candidate;
                return true;
            }
        }
        op = string.Empty;
        return false;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        else if (obj is Scanner scanner)
        {
            return Source.Equals(scanner.Source)
                && Position == scanner.Position;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Source.GetHashCode() ^ Position.GetHashCode();
    }
}