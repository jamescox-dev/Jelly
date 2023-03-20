namespace Jelly.Library;

using Jelly.Ast;
using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Commands;
using Jelly.Values;

public class CoreLibrary : ILibrary
{
    public void LoadIntoScope(IScope scope)
    {
        scope.DefineCommand("var", new SimpleMacro(VarMacro));
        scope.DefineCommand("while", new SimpleMacro(WhileMacro));
    }

    Value VarMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw new ArgError("Expected 'varname'.");
        }
        if (args.Count >= 2)
        {
            var keyword = Evaluator.Shared.Evaluate(scope, args[1].ToDictionaryValue()).ToString();
            if (!keyword.Equals("=", StringComparison.InvariantCulture))
            {
                throw new ArgError($"Expected keyword '=', but found '{keyword}'.");
            }
        }
        if (args.Count > 3)
        {
            var unexpected = Evaluator.Shared.Evaluate(scope, args[3].ToDictionaryValue()).ToString();
            throw new ArgError($"Unexpected value '{unexpected}'.");
        }

        var varnameNode = args[0].ToDictionaryValue();
        var isVariable = varnameNode.ContainsKey(Keywords.Type) && varnameNode[Keywords.Type].Equals(Keywords.Variable);
        var varname = isVariable ? varnameNode[Keywords.Name].ToString() : Evaluator.Shared.Evaluate(scope, varnameNode).ToString();

        return Node.DefineVariable(varname, args.Count == 3 ? args[2].ToDictionaryValue() : Node.Literal(Value.Empty));
    }

    Value WhileMacro(IScope scope, ListValue args)
    {
        if (args.Count == 0)
        {
            throw new ArgError("Expected 'condition'.");
        }
        if (args.Count == 1)
        {
            throw new ArgError("Expected 'body'.");
        }
        if (args.Count > 2)
        {
            var unexpected = Evaluator.Shared.Evaluate(scope, args[2].ToDictionaryValue()).ToString();
            throw new ArgError($"Unexpected value '{unexpected}'.");
        }

        var value = Value.Empty;

        while (Evaluator.Shared.Evaluate(scope, args[0].ToDictionaryValue()).ToDouble() != 0)
        {
            value = Evaluator.Shared.Evaluate(scope, args[1].ToDictionaryValue());
        }

        return value;
    }
}