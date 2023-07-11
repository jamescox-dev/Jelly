namespace Jelly.Commands;

public interface ICommand
{
    Value Invoke(IEnv env, ListValue unevaluatedArgs);
}