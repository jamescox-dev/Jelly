namespace Jelly.Commands.ArgParsers;


public record ArgPatternResult(int Position);

public record ArgPatternSuccess(int Position, IReadOnlyDictionary<string, Value> ArgValues)
    : ArgPatternResult(Position);

public record ArgPatternMissing(int Position, IReadOnlySet<Arg> MissingArgs)
    : ArgPatternResult(Position);

public record ArgPatternUnexpected(int Position) : ArgPatternResult(Position);