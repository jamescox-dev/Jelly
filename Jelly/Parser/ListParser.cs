namespace Jelly.Parser;

using Jelly.Errors;
using Jelly.Parser.Scanning;
using Jelly.Values;
using System.Collections.Immutable;

public class ListParser
{
    static readonly ListItemParser ItemParser = new();

    public ListValue Parse(Scanner scanner)
    {
        var items = ImmutableList.CreateBuilder<Value>();

        scanner.AdvanceWhile(s => s.IsListItemSeparator);
            
        while (!scanner.IsEof)
        {
            var item = ItemParser.Parse(scanner);
            if (item is not null)
            {
                items.Add(item!);
            }
            else
            {
                throw new ParseError("Unexpected input.");
            }
            
            scanner.AdvanceWhile(s => s.IsListItemSeparator);
        }

        return new ListValue(items.ToImmutable());
    }
}