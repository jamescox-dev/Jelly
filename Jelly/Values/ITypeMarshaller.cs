namespace Jelly.Values;

public interface ITypeMarshaller
{
    Value Marshal(object? clrValue);

    object Marshal(Value jellyValue, Type clrType);
}