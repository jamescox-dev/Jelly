namespace Jelly.Evaluator;

internal class DefineCommandEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var name = env.Evaluate(node.GetNode(Keywords.Name)).ToString();

        var uniqueArgNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        var argNames = node[Keywords.ArgNames].ToListValue();
        var argDefaults = node[Keywords.ArgDefaults].ToListValue();

        var requiredArgs = new List<string>();
        foreach (var argName in argNames.Take<Value>(argNames.Count - argDefaults.Count))
        {
            var requiredArgName = rootEvaluator.Evaluate(scope, argName.ToDictionaryValue()).ToString();
            if (uniqueArgNames.Contains(requiredArgName))
            {
                throw Error.Arg($"Argument with name '{requiredArgName}' already defined.");
            }
            uniqueArgNames.Add(requiredArgName);
            requiredArgs.Add(requiredArgName);
        }

        var optionalArgs = new List<(string, Value)>();
        var i = 0;
        foreach(var argName in argNames.Skip<Value>(argNames.Count - argDefaults.Count))
        {
            var defaultValue = rootEvaluator.Evaluate(scope, argDefaults[i++].ToDictionaryValue());
            var optionalArgName = rootEvaluator.Evaluate(scope, argName.ToDictionaryValue()).ToString();
            if (uniqueArgNames.Contains(optionalArgName))
            {
                throw Error.Arg($"Argument with name '{optionalArgName}' already defined.");
            }
            uniqueArgNames.Add(optionalArgName);

            optionalArgs.Add((optionalArgName, defaultValue));
        }

        string? restArgumentName = node.ContainsKey(Keywords.RestArgName) ? rootEvaluator.Evaluate(scope, node[Keywords.RestArgName].ToDictionaryValue()).ToString() : null;
        if (restArgumentName is not null)
        {
            if (uniqueArgNames.Contains(restArgumentName))
            {
                throw Error.Arg($"Argument with name '{restArgumentName}' already defined.");
            }
            uniqueArgNames.Add(restArgumentName);
        }
        scope.DefineCommand(
            name, new UserCommand(requiredArgs, optionalArgs, restArgumentName, node[Keywords.Body].ToDictionaryValue()));

        return Value.Empty;
    }
}