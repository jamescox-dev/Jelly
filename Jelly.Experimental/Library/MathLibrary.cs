namespace Jelly.Experimental;

using Jelly;
using Jelly.Ast;
using Jelly.Commands;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Library;
using Jelly.Values;

public class MathLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("min", new SimpleCommand(MinCmd));
        scope.DefineCommand("max", new SimpleCommand(MaxCmd));
        scope.DefineCommand("inc", new SimpleCommand(IncMacro));
    }

    public Value MinCmd(IScope scope, ListValue args)
    {
        return args.Count == 0 ? NumberValue.Zero : args.Select(n => n.ToDouble()).Min().ToValue();
    }
    
    public Value MaxCmd(IScope scope, ListValue args)
    {
        return args.Count == 0 ? NumberValue.Zero : args.Select(n => n.ToDouble()).Max().ToValue();
    }

    public Value IncMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw Error.Arg("Expected 'variable'.");
        }
        if (args.Count > 2)
        {
            throw Error.Arg("Unexpected arguments after 'amount'.");
        }

        var nameNode = args[0].ToDictionaryValue();

        if (Node.IsVariable(nameNode))
        {
            nameNode = Node.Literal(nameNode[Keywords.Name]);
        }

        var amount = Node.Literal(NumberValue.One);
        if (args.Count == 2)
        {
            amount = args[1].ToDictionaryValue();
        }

        var name = Evaluator.Shared.Evaluate(scope, nameNode).ToString();

        return Node.Assignment(name, Node.Expression(Node.BinOp("add", Node.Variable(name), amount));
    }
}