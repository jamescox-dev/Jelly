namespace Jelly.Commands;

using Jelly.Evaluator;

public class UserCommand : CommandBase
{
    public DictionaryValue Body => _body;

    public IReadOnlyCollection<string> RequiredArgNames => _requiredArgNames;

    public IReadOnlyCollection<string> OptionalArgNames => _optionalArgs.Select(arg => arg.Item1).ToArray();

    public IReadOnlyCollection<Value> OptionalArgDefaultValues => _optionalArgs.Select(arg => arg.Item2).ToArray();

    public string? RestArgName => _restArgName;

    readonly string[] _requiredArgNames;
    readonly (string, Value)[] _optionalArgs;
    readonly string? _restArgName;
    readonly DictionaryValue _body;

    public UserCommand(
        IEnumerable<string> requiredArgNames,
        IEnumerable<(string, Value)> optionalArgs,
        string? restArgName,
        DictionaryValue body)
    {
        _requiredArgNames = requiredArgNames.ToArray();
        _optionalArgs = optionalArgs.ToArray();
        _restArgName = restArgName;
        _body = body;
    }

    public override Value Invoke(IEnvironment env, ListValue unevaluatedArgs)
    {
        return env.RunInNestedScope(() =>
        {
            EnsureArgCountIsValid(env, unevaluatedArgs);

            var args = EvaluateArgs(env, unevaluatedArgs);
            DefineRequiredArgs(env, args);
            DefineOptionalArgs(env, args);
            DefineRestArg(env, args);

            return EvaluateBody(env);
        });
    }

    void DefineRequiredArgs(IEnvironment env, ListValue args)
    {
        foreach (var (argName, argValue) in _requiredArgNames.Zip(args.Take(args.Count)))
        {
            env.CurrentScope.DefineVariable(argName, argValue);
        }
    }

    void DefineOptionalArgs(IEnvironment env, ListValue args)
    {
        var i = _requiredArgNames.Length;
        foreach (var (argName, defaultValue) in _optionalArgs)
        {
            env.CurrentScope.DefineVariable(argName, i < args.Count ? args[i++] : defaultValue);
        }
    }

    void DefineRestArg(IEnvironment env, ListValue args)
    {
        if (_restArgName is not null)
        {
            var nonRestArgCount = _requiredArgNames.Length + _optionalArgs.Length;
            env.CurrentScope.DefineVariable(_restArgName, new ListValue(args.Skip(nonRestArgCount)));
        }
    }

    void EnsureArgCountIsValid(IEnvironment env, ListValue args)
    {
        if (args.Count < _requiredArgNames.Length)
        {
            throw ExpectedArgError(args);
        }
        if (args.Count > _requiredArgNames.Length + _optionalArgs.Length && _restArgName is null)
        {
            throw UnexpectedArgError(env, args);
        }
    }

    // TODO:  This should be a standard error.
    Error ExpectedArgError(ListValue unevaluatedArgs)
    {
        return Error.Arg($"Expected '{_requiredArgNames[unevaluatedArgs.Count]}' argument.");
    }

    // TODO:  This should be a standard error.
    Error UnexpectedArgError(IEnvironment env, ListValue unevaluatedArgs)
    {
        var argValue = env.Evaluate(unevaluatedArgs[_requiredArgNames.Length + _optionalArgs.Length].ToNode());
        throw Error.Arg($"Unexpected argument '{argValue}'.");
    }

    Value EvaluateBody(IEnvironment env)
    {
        try
        {
            return env.Evaluate(_body);
        }
        catch (Return functionReturn)
        {
            return functionReturn.Value;
        }
    }
}