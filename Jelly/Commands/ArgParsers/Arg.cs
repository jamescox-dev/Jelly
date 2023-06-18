namespace Jelly.Commands.ArgParsers;

public record Arg(string Name) {}

public record OptArg(string Name, Value DefaultValue) : Arg(Name)
{
    public OptArg(string name) : this(name, Value.Empty) {}
}

public record RestArg(string Name) : Arg(Name)  {}