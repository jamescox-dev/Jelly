namespace Jelly.Library;

public class MathLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        var mathCmd = new GroupCommand("math");
        mathCmd.AddCommand("max", new WrappedCommand(MaxCmd, typeMarshaller));
        mathCmd.AddCommand("min", new WrappedCommand(MinCmd, typeMarshaller));

        scope.DefineCommand("math", mathCmd);

        scope.DefineVariable("PI", Math.PI.ToValue());
        scope.DefineVariable("TAU", Math.Tau.ToValue());
    }

    double MaxCmd(params double[] values)
    {
        if (!values.Any())
        {
            return 0;
        }

        var max = double.NegativeInfinity;

        foreach (var value in values)
        {
            max = Math.Max(max, value);
        }

        return max;
    }

    double MinCmd(params double[] values)
    {
        if (!values.Any())
        {
            return 0;
        }

        var min = double.PositiveInfinity;

        foreach (var value in values)
        {
            min = Math.Min(min, value);
        }

        return min;
    }

    // TODO:  clamp
    // TODO:  sign
    // TODO:  abs
    // TODO:  acos
    // TODO:  acosh
    // TODO:  cos
    // TODO:  cosh
    // TODO:  asin
    // TODO:  asinh
    // TODO:  sin
    // TODO:  sinh
    // TODO:  atan
    // TODO:  atan2
    // TODO:  atanh
    // TODO:  tan
    // TODO:  tanh
    // TODO:  exp
    // TODO:  pow
    // TODO:  log
    // TODO:  log2
    // TODO:  log10
    // TODO:  sqrt
    // TODO:  cbrt
    // TODO:  floor
    // TODO:  round
    // TODO:  ceil
    // TODO:  trunc
}