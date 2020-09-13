namespace Larnaca.Blueprints
{
    public class OperationResult : IOperationResult
    {
        public OperationResult() { }
        public OperationResult(string? statusMessage) : this(1, statusMessage) { }
        public OperationResult(int statusCode, string? statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }
        public OperationResult(IOperationResult op) : this(op?.StatusCode ?? 0, op?.StatusMessage) { }
        public int StatusCode { get; set; }
        public string? StatusMessage { get; set; }
        public override string ToString()
        {
            return StatusCode.ToString() + " " + StatusMessage;
        }
    }
    public class OperationResult<T> : OperationResult, IOperationResult<T>
    {
#pragma warning disable CS8601 // Possible null reference assignment of Data, we don't know if it's value or reference type and hence cannot mark it as nullable
        public OperationResult() : base()
        {
            Data = default;
        }
        public OperationResult(string? statusMessage) : this(1, statusMessage)
        {
            Data = default;
        }
        public OperationResult(int statusCode, string? statusMessage) : base(statusCode, statusMessage)
        {
            Data = default;
        }
        public OperationResult(string? statusMessage, T data) : this(1, statusMessage, data)
        {
            Data = default;
        }
        public OperationResult(int statusCode, string? statusMessage, T data) : this(statusCode, statusMessage)
        {
            Data = data;
        }
        public OperationResult(IOperationResult op) : this(op?.StatusCode ?? 0, op?.StatusMessage)
        {
            Data = default;
        }
#pragma warning restore CS8601 // Possible null reference assignment of Data, we don't know if it's value or reference type and hence cannot mark it as nullable
        public OperationResult(IOperationResult op, T data) : this(op?.StatusCode ?? 0, op?.StatusMessage)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
