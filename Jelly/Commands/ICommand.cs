namespace Jelly.Commands;

public interface ICommand
{
    Value Invoke(IEnvironment env, ListValue unevaluatedArgs);
}