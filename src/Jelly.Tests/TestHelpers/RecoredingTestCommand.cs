namespace Jelly.Tests.TestHelpers;

public class RecordingTestCommand : CommandBase
{
    public int Invocations { get; set; }
    public List<ListValue> RecordedArguments { get; set; } = new();

    public override Value Invoke(IEnv env, ListValue args)
    {
        args = EvaluateArgs(env, args);
        RecordedArguments.Add(args);
        ++Invocations;
        return Invocations.ToValue();
    }
}