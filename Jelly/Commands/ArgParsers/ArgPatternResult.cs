namespace Jelly.Commands.ArgParsers;


public record ArgPatternResult(int Position);

public record ArgPatternSuccess(int Position, IReadOnlyDictionary<string, Value> ArgValues)
    : ArgPatternResult(Position);

public record ArgPatternResultMissing(int Position, IReadOnlySet<Arg> MissingArgs)
    : ArgPatternResult(Position);

public record ArgPatternResultUnexpected(int Position) : ArgPatternResult(Position);