namespace Larnaca.Blueprints
{
    public interface IOperationResultWithLogs : IOperationResult, ILogs { }
    public interface IOperationResultWithLogs<T> : IOperationResult<T>, ILogs { }
}