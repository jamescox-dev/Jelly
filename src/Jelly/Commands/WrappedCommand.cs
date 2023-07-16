namespace Jelly.Commands;

public class WrappedCommand : CommandBase
{
    readonly ITypeMarshaller _typeMarshaller;
    readonly Delegate _wrappedDelegate;
    readonly Type[] _argTypes;
    readonly string[] _argNames;
    readonly object?[] _optionalArgDefaultValues;
    readonly int _minPositionalArgCount;
    readonly int _maxPositionalArgsCount;
    readonly Type? _paramsArgType;

    public WrappedCommand(Delegate wrappedDelegate, ITypeMarshaller typeMarshaller)
    {
        _wrappedDelegate = wrappedDelegate;
        _typeMarshaller = typeMarshaller;

        var argTypes = new List<Type>();
        var argNames = new List<string>();
        var optionalArgDefaultValues = new List<object?>();
        var minPositionalArgCount = 0;
        var maxPositionalArgCount = 0;
        Type? paramsArgType = null;
        foreach (var param in _wrappedDelegate.Method.GetParameters())
        {
            argTypes.Add(param.ParameterType);
            argNames.Add(param.Name ?? string.Empty);
            if (param.IsOptional)
            {
                optionalArgDefaultValues.Add(param.DefaultValue);
                ++maxPositionalArgCount;
            }
            else
            {
                if (Attribute.IsDefined(param, typeof(ParamArrayAttribute)))
                {
                    paramsArgType = param.ParameterType.GetElementType();
                }
                else
                {
                    ++maxPositionalArgCount;
                    ++minPositionalArgCount;
                }
            }
        }
        _argTypes = argTypes.ToArray();
        _argNames = argNames.ToArray();
        _optionalArgDefaultValues = optionalArgDefaultValues.ToArray();
        _minPositionalArgCount = minPositionalArgCount;
        _maxPositionalArgsCount = maxPositionalArgCount;
        _paramsArgType = paramsArgType;
    }

    // TODO:  In need of some serious refactoring.
    public override Value Invoke(IEnv env, ListValue unevaluatedArgs)
    {
        EnsureArgCountIsValid(env, unevaluatedArgs);
        var args = EvaluateArgs(env, unevaluatedArgs);

        var clrArgs = new object?[_maxPositionalArgsCount + (_paramsArgType is not null ? 1 : 0)];
        var i = 0;
        foreach (var arg in args.Take(_maxPositionalArgsCount))
        {
            clrArgs[i] = _typeMarshaller.Marshal(arg, _argTypes[i]);
            ++i;
        }
        while (i < _maxPositionalArgsCount)
        {
            clrArgs[i] = _optionalArgDefaultValues[i - _minPositionalArgCount];
            ++i;
        }
        if (_paramsArgType is not null)
        {
            var extraCount = args.Count - _maxPositionalArgsCount;
            var extraParams = Array.CreateInstance(_paramsArgType, extraCount);

            for (var j = 0; j < extraCount; ++j)
            {
                extraParams.SetValue(_typeMarshaller.Marshal(args[i], _paramsArgType), j);
                ++i;
            }

            clrArgs[_maxPositionalArgsCount] = extraParams;
        }

        var result = _wrappedDelegate.DynamicInvoke(clrArgs);
        return _typeMarshaller.Marshal(result);
    }

    void EnsureArgCountIsValid(IEnv env, ListValue unevaluatedArgs)
    {
        if (unevaluatedArgs.Count < _minPositionalArgCount)
        {
            throw ExpectedArgError(unevaluatedArgs);
        }
        if (unevaluatedArgs.Count > _maxPositionalArgsCount && _paramsArgType is null)
        {
            throw UnexpectedArgError(env, unevaluatedArgs);
        }
    }

    // TODO:  This should be a standard error.
    Error ExpectedArgError(ListValue unevaluatedArgs)
    {
        return Error.Arg($"Expected '{_argNames[unevaluatedArgs.Count]}' argument.");
    }

    // TODO:  This should be a standard error.
    Error UnexpectedArgError(IEnv env, ListValue unevaluatedArgs)
    {
        var argValue = env.Evaluate(unevaluatedArgs[_minPositionalArgCount].ToNode());
        throw Error.Arg($"Unexpected argument '{argValue}'.");
    }
}