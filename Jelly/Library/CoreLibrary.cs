namespace Jelly.Library;

using Jelly.Errors;
using Jelly.Evaluator;
using Jelly.Commands;
using Jelly.Values;

public class CoreLibrary
{
    static readonly StringValue TypeKeyword = new StringValue("type");
    static readonly StringValue VariableKeyword = new StringValue("variable");
    static readonly StringValue NameKeyword = new StringValue("name");

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
        var isVariable = varnameNode.ContainsKey(TypeKeyword) && varnameNode[TypeKeyword].Equals(VariableKeyword);
        var varname = isVariable ? varnameNode[NameKeyword].ToString() : Evaluator.Shared.Evaluate(scope, varnameNode).ToString();
        var value = args.Count == 3 ? Evaluator.Shared.Evaluate(scope, args[2].ToDictionaryValue()) : Value.Empty;

        scope.DefineVariable(varname, value);

        return value;
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