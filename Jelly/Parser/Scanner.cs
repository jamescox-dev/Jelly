namespace Jelly.Parser;

public class Scanner
{
    public string Source { get; private set; }

    public int Position { get; private set; }

    public Scanner(string source)
    {
        Source = source;
        Position = 0;
    }

    public Scanner(string source, int position)
    {
        Source = source;
        Position = position;
    }

    public void Advance(int amount=1)
    {
        Position += amount;
    }

    public override bool Equals(object? obj)
    {
        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }
        else if (obj is Scanner scanner)
        {
            return this.Source.Equals(scanner.Source) 
                && this.Position == scanner.Position;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Source.GetHashCode() ^ Position.GetHashCode();
    }
}