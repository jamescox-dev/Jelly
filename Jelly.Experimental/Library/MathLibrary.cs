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
        var typeMarshaller = new TypeMarshaller();

        scope.DefineCommand("min", new WrappedCommand(MinCmd, typeMarshaller));
        scope.DefineCommand("max", new WrappedCommand(MaxCmd, typeMarshaller));
        scope.DefineCommand("inc", new SimpleMacro(IncMacro));
    }

    public double MinCmd(params double[] numbers)
    {
        if (numbers.Length == 0)
        {
            return 0.0;
        }
        return numbers.Min();
    }

    public double MaxCmd(params double[] numbers)
    {
        if (numbers.Length == 0)
        {
            return 0.0;
        }
        return numbers.Max();
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

        return Node.Assignment(name, Node.BinOp("add", Node.Variable(name), amount));
    }
}