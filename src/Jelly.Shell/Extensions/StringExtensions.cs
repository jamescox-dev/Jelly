namespace Jelly.Shell.Extensions;

public static class StringExtensions
{
    static readonly IReadOnlyList<string> LineEndings = new[] {
        "\r\n", "\n", "\r"
    };

    public static (int, int) IndexToLineAndColumn(this string str, int index)
    {
        index = Math.Clamp(index, 0, str.Length);
        var lineIndex = 1;

        var previousLine = new Substring(str, 0, 0);
        foreach (var line in str.SplitLines())
        {
            if (index >= line.Start && index <= line.End)
            {
                return (lineIndex, (index - line.Start) + 1);
            }
            else if (index >= previousLine.Start && index < line.Start)
            {
                return (lineIndex - 1, (index - previousLine.Start) + 1);
            }
            ++lineIndex;
            previousLine = line;
        }

        return (lineIndex - 1, (index - previousLine.Start) + 1);
    }

    public static IEnumerable<UnderlinedText> Underline(this string str, int start, int end)
    {
        start = Math.Max(start, 0);
        end = Math.Min(end, str.Length);
        if (start == end)
        {
            ++end;
        }

        if (start == str.Length)
        {
            var line = str.SplitLines().Last().ToString();
            yield return new UnderlinedText(line, new string(' ', line.Length) + "^");
        }
        else
        {
            foreach (var line in str.SplitLines())
            {
                if (line.Start < start && line.End < start)
                {
                    continue;
                }
                else if (line.Start >= end && line.End >= end)
                {
                    break;
                }
                var underlineStart = Math.Max(start - line.Start, 0);
                var underlineEnd = Math.Min(end - line.Start, line.End - line.Start);
                var indent = new string(' ', underlineStart);
                var underline = new string('^', underlineEnd - underlineStart);
                yield return new UnderlinedText(line.ToString(), indent + underline);
            }
        }
    }

    public static IEnumerable<Substring> SplitLines(this string str)
    {
        var lineStart = 0;
        var position = 0;
        while (position < str.Length)
        {
            var lineEndingLen = FindLineEnding(str, ref position);
            if (lineEndingLen != 0)
            {
                yield return new Substring(str, lineStart, position);
                position += lineEndingLen;
                lineStart = position;
            }
        }
        yield return new Substring(str, lineStart, position);
    }

    static int FindLineEnding(string str, ref int position)
    {
        while (position < str.Length)
        {
            foreach (var lineEnding in LineEndings)
            {
                var lineEndingLen = lineEnding.Length;
                if (IsLineEndingAt(str, position, lineEnding))
                {
                    return lineEndingLen;
                }
            }
            ++position;
        }
        return 0;
    }

    private static bool IsLineEndingAt(string str, int position, string lineEnding)
    {
        var substringEnd = position + lineEnding.Length;
        return substringEnd <= str.Length && str[position..substringEnd] == lineEnding;
    }
}