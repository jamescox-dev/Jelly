namespace Jelly.Evaluator;

internal class TryEvaluator : IEvaluator
{
    public Value Evaluate(IEnvironment env, DictionaryValue node)
    {
        var body = node.GetNode(Keywords.Body);
        var finallyBody = node.GetNodeOrNull(Keywords.Finally);
        var errorHandlers = node.GetList(Keywords.ErrorHandlers);

        Value result = Value.Empty;
        try
        {
            result = env.Evaluate(body);
        }
        catch (Error error)
        {
            var errorHandler = GetErrorHandlerForError(env, errorHandlers, error);
            if (errorHandler is not null)
            {
                result = env.Evaluate(errorHandler);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            if (finallyBody is not null)
            {
                result = env.Evaluate(finallyBody);
            }
        }

        return result;
    }

    static DictionaryValue? GetErrorHandlerForError(IEnvironment env, ListValue errorHandlers, Error error)
    {
        foreach (var errorHandler in errorHandlers)
        {
            var errorHandlerTuple = errorHandler.ToListValue();
            var errorTypePattern = env.Evaluate(errorHandlerTuple[0].ToNode()).ToString();
            if (error.IsType(errorTypePattern))
            {
                var errorHandlerBody = errorHandlerTuple[1].ToNode();
                return errorHandlerBody;
            }
        }
        return null;
    }
}