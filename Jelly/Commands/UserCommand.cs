namespace Jelly.Commands;

using Jelly.Evaluator;

public class UserCommand : ICommand
{
    public EvaluationFlags EvaluationFlags => Commands.EvaluationFlags.Arguments;

    public DictionaryValue Body => _body;

    public IReadOnlyCollection<string> RequiredArgumentNames => _requiredArguments;

    public IReadOnlyCollection<string> OptionalArgumentNames => _optionalArguments.Select(arg => arg.Item1).ToArray();

    public IReadOnlyCollection<Value> OptionalArgumentDefaultValues => _optionalArguments.Select(arg => arg.Item2).ToArray();

    public string? RestArgumentName => _restArgument;

    readonly string[] _requiredArguments;
    readonly (string, Value)[] _optionalArguments;
    readonly string? _restArgument;
    readonly DictionaryValue _body;

    public UserCommand(IEnumerable<string> requiredArguments, IEnumerable<(string, Value)> optionalArguments, string? restArgument, DictionaryValue body)
    {
        _requiredArguments = requiredArguments.ToArray();
        _optionalArguments = optionalArguments.ToArray();
        _restArgument = restArgument;
        _body = body;
    }

    public Value Invoke(IScope scope, ListValue args)
    {
        var commandScope = new Scope(scope);

        if (args.Count < _requiredArguments.Length)
        {
            throw Error.Arg($"Expected '{_requiredArguments[args.Count]}' argument.");
        }
        if (args.Count > _requiredArguments.Length + _optionalArguments.Length && _restArgument is null)
        {
            throw Error.Arg($"Unexpected argument '{args[_requiredArguments.Length + _optionalArguments.Length]}'.");
        }

        var i = 0;
        foreach (var arg in _requiredArguments)
        {
            commandScope.DefineVariable(arg, args[i++]);
        }
        foreach (var (arg, defaultValue) in _optionalArguments)
        {
            commandScope.DefineVariable(arg, i < args.Count ? args[i++] : defaultValue);
        }
        if (_restArgument is not null)
        {
            commandScope.DefineVariable(_restArgument!, new ListValue(args.Skip(i)));
        }

        try
        {
            // TODO:  To change this.
            return Value.Empty;//Evaluator.Shared.Evaluate(commandScope, _body);
        }
        catch (Return @return)
        {
            return @return.Value;
        }
    }
}