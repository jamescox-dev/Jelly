namespace Jelly.Evaluator;

internal class DefineCommandEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var name = env.Evaluate(node.GetNode(Keywords.Name)).ToString();
        var argNames = node.GetList(Keywords.ArgNames);
        var argDefaults = node.GetList(Keywords.ArgDefaults);
        var body = node.GetNode(Keywords.Body);
        var restArgNameNode = node.GetNodeOrNull(Keywords.RestArgName);

        var usedArgNames = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);

        var requiredArgNames = GetRequiredArgNames(env, argNames, argDefaults.Count, usedArgNames);
        var optionalArgs = GetOptionalArgs(env, argNames, argDefaults, usedArgNames);
        var restArgName = GetRestArgumentName(env, restArgNameNode, usedArgNames);

        env.CurrentScope.DefineCommand(name, new UserCommand(requiredArgNames, optionalArgs, restArgName, body));

        return Value.Empty;
    }

    static List<string> GetRequiredArgNames(
        IEnvironment env, ListValue argNames, int numberOfArgsWithDefaults, HashSet<string> usedArgNames)
    {
        var requiredArgs = new List<string>();
        foreach (var argName in argNames.Take<Value>(argNames.Count - numberOfArgsWithDefaults))
        {
            var requiredArgName = env.Evaluate(argName.ToNode()).ToString();
            RecordArgName(requiredArgName, usedArgNames);
            requiredArgs.Add(requiredArgName);
        }

        return requiredArgs;
    }

    static List<(string, Value)> GetOptionalArgs(
        IEnvironment env, ListValue argNames, ListValue argDefaults, HashSet<string> usedArgNames)
    {
        var optionalArgs = new List<(string, Value)>();
        foreach (var (argName, argValue) in argNames.Skip<Value>(argNames.Count - argDefaults.Count).Zip(argDefaults))
        {
            var optionalArgName = env.Evaluate(argName.ToNode()).ToString();
            var defaultValue = env.Evaluate(argValue.ToNode());
            RecordArgName(optionalArgName, usedArgNames);
            optionalArgs.Add((optionalArgName, defaultValue));
        }

        return optionalArgs;
    }

    static string? GetRestArgumentName(IEnvironment env, DictionaryValue? restArgNameNode, HashSet<string> usedArgNames)
    {
        if (restArgNameNode is not null)
        {
            var restArgName = env.Evaluate(restArgNameNode).ToString();
            RecordArgName(restArgName, usedArgNames);
            return restArgName;
        }

        return null;
    }

    static void RecordArgName(string argName, HashSet<string> usedArgNames)
    {
        if (usedArgNames.Contains(argName))
        {
            throw Error.Arg($"Argument with name '{argName}' already defined.");
        }
        usedArgNames.Add(argName);
    }
}