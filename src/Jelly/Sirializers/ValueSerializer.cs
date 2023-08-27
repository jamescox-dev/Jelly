namespace Jelly.Values;

using System.Text;

public static class ValueSerializer
{
    public static string Escape(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            var quote = ScannerConfig.Default.Quotes.First();
            return $"{quote}{quote}";
        }

        var scanner = new Scanner(str);
        var specialCharacterPositions = new SortedDictionary<int, char>();
        var quotesInString = new HashSet<char>();
        var escapeCharRun = 0;
        var quoteBalance = 0;

        while (!scanner.IsEof)
        {
            if (scanner.IsQuote && escapeCharRun % 2 == 0)
            {
                quotesInString.Add((char)scanner.CurrentCharacter!);
            }
            else if (scanner.IsNestingQuoteBegin && escapeCharRun % 2 == 0)
            {
                ++quoteBalance;
            }
            else if (scanner.IsNestingQuoteEnd && escapeCharRun % 2 == 0)
            {
                --quoteBalance;
            }
            if (scanner.IsSpecialCharacter)
            {
                specialCharacterPositions[scanner.Position] = (char)scanner.CurrentCharacter!;
            }
            if (scanner.IsEscapeCharacter)
            {
                ++escapeCharRun;
            }
            else
            {
                escapeCharRun = 0;
            }
            scanner.Advance();
        }

        if (specialCharacterPositions.Count == 0)
        {
            return scanner.Source;
        }
        if (specialCharacterPositions.Count == 1 && !char.IsWhiteSpace(specialCharacterPositions.Values.First()))
        {
            return SimpleEscape(scanner.Source, specialCharacterPositions);
        }
        if (quoteBalance == 0 && quotesInString.Count == ScannerConfig.Default.Quotes.Count)
        {
            return NestingQuoteEscape(scanner.Source);
        }

        foreach (var quote in ScannerConfig.Default.Quotes.ToList())
        {
            if (!quotesInString.Contains(quote))
            {
                return QuoteEscape(scanner.Source, specialCharacterPositions, quote);
            }
        }

        return QuoteEscape(scanner.Source, specialCharacterPositions, ScannerConfig.Default.Quotes.First());
    }

    static string SimpleEscape(string str, IReadOnlyDictionary<int, char> specialCharacterPositions)
    {
        var escapedStr = new StringBuilder();

        var lastPos = 0;
        foreach (var pos in specialCharacterPositions.Keys)
        {
            escapedStr.Append(str[lastPos..pos]);
            escapedStr.Append(ScannerConfig.Default.EscapeCharacter);
            lastPos = pos;
        }
        escapedStr.Append(str[lastPos..]);

        return escapedStr.ToString();
    }

    static string NestingQuoteEscape(string str)
    {
        return $"[{str}]";
    }

    static string QuoteEscape(string str, IReadOnlyDictionary<int, char> specialCharacterPositions, char quote)
    {
        var escapedStr = new StringBuilder();
        escapedStr.Append(quote);

        var lastPos = 0;
        foreach (var pos in specialCharacterPositions.Keys)
        {
            var ch = specialCharacterPositions[pos];
            if (ch == ScannerConfig.Default.ScriptBegin || ch == ScannerConfig.Default.EscapeCharacter || ch == quote)
            {
                escapedStr.Append(str[lastPos..pos]);
                escapedStr.Append(ScannerConfig.Default.EscapeCharacter);
                lastPos = pos;
            }
        }
        escapedStr.Append(str[lastPos..]);

        escapedStr.Append(quote);
        return escapedStr.ToString();
    }
}