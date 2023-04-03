namespace Jelly.Commands;

using Jelly.Errors;
using Jelly.Values;

public class WrappedCommand : ICommand
{
    readonly ITypeMarshaller _typeMashaller;
    readonly Delegate _wrappedDelegate;
    readonly Type[] _argumentTypes;
    readonly string[] _argumentNames;
    readonly object?[] _optionalArgumentDefaultValues;
    readonly int _requiredArgumentCount;
    readonly int _maxArgumentCount;
    readonly Type? _paramsType;

    public EvaluationFlags EvaluationFlags => EvaluationFlags.Arguments;

    public WrappedCommand(Delegate wrappedDelegate, ITypeMarshaller typeMarshaller)
    {
        _wrappedDelegate = wrappedDelegate;
        _typeMashaller = typeMarshaller;
        
        var argumentTypes = new List<Type>();
        var argumentNames = new List<string>();
        var optionalArgumentDefaultValues = new List<object?>();
        var requiredArgumentCount = 0;
        var maxArgumentCount = 0;
        Type? paramsType = null;
        foreach (var param in _wrappedDelegate.Method.GetParameters())
        {
            argumentTypes.Add(param.ParameterType);
            argumentNames.Add(param.Name ?? "");
            if (param.IsOptional)
            {
                optionalArgumentDefaultValues.Add(param.DefaultValue);
                ++maxArgumentCount;
            }
            else
            {
                if (Attribute.IsDefined(param, typeof(ParamArrayAttribute)))
                {
                    paramsType = param.ParameterType.GetElementType();
                }
                else
                {
                    ++maxArgumentCount;
                    ++requiredArgumentCount;
                }
            }
        }
        _argumentTypes = argumentTypes.ToArray();
        _argumentNames = argumentNames.ToArray();
        _optionalArgumentDefaultValues = optionalArgumentDefaultValues.ToArray();
        _requiredArgumentCount = requiredArgumentCount;
        _maxArgumentCount = maxArgumentCount;
        _paramsType = paramsType;
    }

    public Value Invoke(IScope scope, ListValue args)
    {
        if (args.Count > _maxArgumentCount && _paramsType is null)
        {
            throw Error.Arg($"Unexpected argument '{args[_requiredArgumentCount]}'.");
        }
        if (args.Count < _requiredArgumentCount)
        {
            throw Error.Arg($"Expected '{_argumentNames[args.Count]}' argument.");
        }

        var clrArgs = new object?[_maxArgumentCount + (_paramsType is not null ? 1 : 0)];
        var i = 0;
        foreach (var arg in args.Take(_maxArgumentCount))
        {
            clrArgs[i] = _typeMashaller.Marshal(arg, _argumentTypes[i]);
            ++i;
        }
        while (i < _maxArgumentCount)
        {
            clrArgs[i] = _optionalArgumentDefaultValues[i - _requiredArgumentCount];
            ++i;
        }
        if (_paramsType is not null)
        {
            var extraCount = args.Count - _maxArgumentCount;
            var extraParams = Array.CreateInstance(_paramsType, extraCount);
        
            for (var j = 0; j < extraCount; ++j)
            {
                extraParams.SetValue(_typeMashaller.Marshal(args[i], _paramsType), j);
                ++i;
            }

            clrArgs[_maxArgumentCount] = extraParams;
        }

        var result = _wrappedDelegate.DynamicInvoke(clrArgs);

        return _typeMashaller.Marshal(result);
    }
}