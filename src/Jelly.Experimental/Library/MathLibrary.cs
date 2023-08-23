namespace Jelly.Experimental.Library;

using Jelly.Commands.ArgParsers;

public class MathLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        var mathCmd = scope.GetGroupCommandOrDefineEmpty("math");
        mathCmd.AddCommand("clamp", new WrappedCommand((Func<double, double, double, double>)Math.Clamp, TypeMarshaller.Shared));
        mathCmd.AddCommand("sign", new WrappedCommand((Func<double, int>)Math.Sign, TypeMarshaller.Shared));
        mathCmd.AddCommand("abs", new WrappedCommand((Func<double, double>)Math.Abs, TypeMarshaller.Shared));

        mathCmd.AddCommand("acos", new WrappedCommand(Math.Acos, TypeMarshaller.Shared));
        mathCmd.AddCommand("acosh", new WrappedCommand(Math.Acosh, TypeMarshaller.Shared));
        mathCmd.AddCommand("cos", new WrappedCommand(Math.Cos, TypeMarshaller.Shared));
        mathCmd.AddCommand("cosh", new WrappedCommand(Math.Cosh, TypeMarshaller.Shared));
        mathCmd.AddCommand("asin", new WrappedCommand(Math.Asin, TypeMarshaller.Shared));
        mathCmd.AddCommand("asinh", new WrappedCommand(Math.Asinh, TypeMarshaller.Shared));
        mathCmd.AddCommand("sin", new WrappedCommand(Math.Sin, TypeMarshaller.Shared));
        mathCmd.AddCommand("sinh", new WrappedCommand(Math.Sinh, TypeMarshaller.Shared));
        mathCmd.AddCommand("atan", new WrappedCommand(Math.Atan, TypeMarshaller.Shared));
        mathCmd.AddCommand("atan2", new WrappedCommand(Math.Atan2, TypeMarshaller.Shared));
        mathCmd.AddCommand("atanh", new WrappedCommand(Math.Atanh, TypeMarshaller.Shared));
        mathCmd.AddCommand("tan", new WrappedCommand(Math.Tan, TypeMarshaller.Shared));
        mathCmd.AddCommand("tanh", new WrappedCommand(Math.Tanh, TypeMarshaller.Shared));

        mathCmd.AddCommand("exp", new WrappedCommand(Math.Exp, TypeMarshaller.Shared));
        mathCmd.AddCommand("pow", new WrappedCommand(Math.Pow, TypeMarshaller.Shared));
        mathCmd.AddCommand("log", new WrappedCommand((Func<double, double>)Math.Log, TypeMarshaller.Shared));
        mathCmd.AddCommand("log2", new WrappedCommand(Math.Log2, TypeMarshaller.Shared));
        mathCmd.AddCommand("log10", new WrappedCommand(Math.Log10, TypeMarshaller.Shared));
        mathCmd.AddCommand("sqrt", new WrappedCommand(Math.Sqrt, TypeMarshaller.Shared));
        mathCmd.AddCommand("cbrt", new WrappedCommand(Math.Cbrt, TypeMarshaller.Shared));

        mathCmd.AddCommand("floor", new WrappedCommand((Func<double, double>)Math.Floor, TypeMarshaller.Shared));
        mathCmd.AddCommand("round", new WrappedCommand(Round, TypeMarshaller.Shared));
        mathCmd.AddCommand("ceil", new WrappedCommand((Func<double, double>)Math.Ceiling, TypeMarshaller.Shared));
        mathCmd.AddCommand("trunc", new WrappedCommand((Func<double, double>)Math.Truncate, TypeMarshaller.Shared));
    }

    double Round(double n, int digits=0) => Math.Round(n, digits);
}