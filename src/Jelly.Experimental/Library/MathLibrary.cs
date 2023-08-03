namespace Jelly.Experimental.Library;

using Jelly.Commands.ArgParsers;

public class MathLibrary : ILibrary
{

    public void LoadIntoScope(IScope scope)
    {
        var typeMarshaller = new TypeMarshaller();

        var mathCmd = scope.GetGroupCommandOrDefineEmpty("math");
        mathCmd.AddCommand("clamp", new WrappedCommand((Func<double, double, double, double>)Math.Clamp, typeMarshaller));
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