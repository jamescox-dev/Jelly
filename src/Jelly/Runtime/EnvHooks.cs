namespace Jelly.Runtime;

public record class EnvHooks(Action<DictValue>? OnEvaluate=null);