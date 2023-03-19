namespace Jelly.Experimental;

using Jelly;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Values;

public class MathLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("min", new SimpleCommand(Min));
        scope.DefineCommand("max", new SimpleCommand(Max));
    }

    public Value Min(IScope scope, ListValue args)
    {
        return args.Count == 0 ? NumberValue.Zero : args.Select(n => n.ToDouble()).Min().ToValue();
    }
    
    public Value Max(IScope scope, ListValue args)
    {
        return args.Count == 0 ? NumberValue.Zero : args.Select(n => n.ToDouble()).Max().ToValue();
    }
}