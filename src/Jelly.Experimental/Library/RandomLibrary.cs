namespace Jelly.Experimental.Library;

public class RandomLibrary : ILibrary
{
    // TODO:  Add IRandomSource/seed.

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        var randomCmd = new GroupCommand("random");

        randomCmd.AddCommand("int", new WrappedCommand(RandomIntCmd, typeMarshaller));

        scope.DefineCommand("random", randomCmd);
    }

    public static int RandomIntCmd(int a, int b = 1)
    {
        var min = Math.Min(a, b);
        var max = Math.Max(a, b) + 1;
        return Random.Shared.Next(min, max);
    }

    // TODO:  Random Choice
    // TODO:  Random Real
}
