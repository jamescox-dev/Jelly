namespace Jelly.Tests.TestHelpers;

public class TestArgPattern : IArgPattern
{
    public ArgPatternResult Result { get; set; }

    public int PositionPassedToMatch { get; set; } = -1;

    public ListValue? ArgsPassedToMatch { get; set; }

    public TestArgPattern(ArgPatternResult result)
    {
        Result = result;
    }

    public ArgPatternResult Match(int position, ListValue args)
    {
        PositionPassedToMatch = position;
        ArgsPassedToMatch = args;

        return Result;
    }
}