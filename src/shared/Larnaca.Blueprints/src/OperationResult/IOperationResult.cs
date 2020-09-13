namespace Larnaca.Blueprints
{
    public interface IOperationResult
    {
        int StatusCode { get; set; }
        string? StatusMessage { get; set; }
    }
    public interface IOperationResult<T> : IOperationResult
    {
        T Data { get; set; }
    }
}