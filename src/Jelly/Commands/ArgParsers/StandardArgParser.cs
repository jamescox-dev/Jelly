namespace Jelly.Commands.ArgParsers;

public class StandardArgParser : IArgParser
{
    readonly Arg[] _requiredArgs;
    readonly OptArg[] _optionalArgs;
    readonly RestArg? _restArg;
    readonly int _minArgs;
    readonly int _maxArgs;

    public StandardArgParser(params Arg[] args)
    {
        var requiredArgs = new List<Arg>();
        var optionalArgs = new List<OptArg>();
        foreach (var arg in args)
        {
            if (arg is OptArg optArg)
            {
                optionalArgs.Add(optArg);
            }
            else if (arg is RestArg restArg)
            {
                _restArg = restArg;
            }
            else
            {
                requiredArgs.Add(arg);
            }
        }
        _requiredArgs = requiredArgs.ToArray();
        _optionalArgs = optionalArgs.ToArray();
        _minArgs = _requiredArgs.Length;
        _maxArgs = _restArg is not null ? int.MaxValue : _minArgs + _optionalArgs.Length;
    }

    public DictValue Parse(string commandName, ListValue args)
    {
        EnsureCorrectArgumentCount(commandName, args);

        var parsedArgs = ImmutableSortedDictionary.CreateBuilder<Value, Value>();

        ParseRequiredArguments(args, parsedArgs);
        ParseOptionalArguments(args, parsedArgs);
        ParseRestArguments(args, parsedArgs);

        return new DictValue(parsedArgs.ToImmutable());
    }

    void ParseRequiredArguments(ListValue args, ImmutableSortedDictionary<Value, Value>.Builder parsedArgs)
    {
        for (var i = 0; i < _requiredArgs.Length; ++i)
        {
            var reqArgName = _requiredArgs[i].Name.ToValue();
            var arg = args[i];
            parsedArgs.Add(reqArgName, arg);
        }
    }

    void ParseOptionalArguments(ListValue args, ImmutableSortedDictionary<Value, Value>.Builder parsedArgs)
    {
        for (var i = 0; i < _optionalArgs.Length; ++i)
        {
            var argIndex = _minArgs + i;
            var optArgName = _optionalArgs[i].Name.ToValue();
            var optArgValue = argIndex < args.Count ? args[argIndex] : _optionalArgs[i].DefaultValue;

            parsedArgs.Add(optArgName, optArgValue);
        }
    }

    void ParseRestArguments(ListValue args, ImmutableSortedDictionary<Value, Value>.Builder parsedArgs)
    {
        var restArgStartIndex = _minArgs + _optionalArgs.Length;
        if (_restArg is not null)
        {
            parsedArgs.Add(_restArg.Name.ToValue(), args.Skip(restArgStartIndex).ToValue());
        }
    }

    void EnsureCorrectArgumentCount(string commandName, ListValue args)
    {
        var argCount = args.Count;
        if (argCount > _maxArgs)
        {
            if (_optionalArgs.Any())
            {
                throw new UnexpectedArgError(commandName, _minArgs, _maxArgs, argCount);
            }
            throw new UnexpectedArgError(commandName, _maxArgs, argCount);
        }
        else if (argCount < _minArgs)
        {
            throw new MissingArgError(commandName, _requiredArgs.Skip(argCount));
        }
    }
}