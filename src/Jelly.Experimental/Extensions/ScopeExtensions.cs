namespace Jelly.Experimental.Extensions;

public static class ScopeExtensions
{
    public static GroupCommand GetGroupCommandOrDefineEmpty(this IScope scope, string name)
    {
        try
        {
            if (scope.GetCommand(name) is not GroupCommand command)
            {
                throw new InvalidOperationException($"{name} is not a GroupCommand.");
            }
            return command;
        }
        catch (NameError)
        {
            var command = new GroupCommand(name);
            scope.DefineCommand(name, command);
            return command;
        }
    }
}