using System.Collections.Generic;
using System.Text;

namespace Larnaca.Blueprints
{
    public class OperationResultWithLogs : OperationResult, IOperationResultWithLogs
    {
        public List<string> Logs { get; set; } = new List<string>();
        public OperationResultWithLogs() : base() { }
        public OperationResultWithLogs(string? statusMessage) : this(1, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string? statusMessage) : base(statusCode, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string? statusMessage, IEnumerable<string>? logs) : this(statusCode, statusMessage)
        {
            if (logs != null)
            {
                if (logs is List<string> logsList)
                {
                    Logs = logsList;
                }
                else
                {
                    Logs = new List<string>(logs);
                }
            }
        }
        public OperationResultWithLogs(IOperationResult op) : this(op?.StatusCode ?? 0, op?.StatusMessage)
        {
            if (op is ILogs iLogsOp)
            {
                Logs = iLogsOp.Logs ?? new List<string>();
            }
        }
        public OperationResultWithLogs(ILogs logs) : this()
        {
            Logs = logs?.Logs ?? new List<string>();
            if (logs is IOperationResult op)
            {
                StatusCode = op.StatusCode;
                StatusMessage = op.StatusMessage;
            }
        }
        public OperationResultWithLogs(IOperationResultWithLogs op) : this(op?.StatusCode ?? 0, op?.StatusMessage, op?.Logs) { }
        public override string ToString()
        {
            StringBuilder theReturn = new StringBuilder();
            foreach (var currentLog in Logs)
            {
                theReturn.AppendLine(currentLog);
            }
            theReturn.Append(StatusCode);
            theReturn.Append(" ");
            theReturn.Append(StatusMessage);
            return theReturn.ToString();
        }
    }
    public class OperationResultWithLogs<T> : OperationResult<T>, IOperationResultWithLogs<T>
    {
        public List<string> Logs { get; set; } = new List<string>();
        public OperationResultWithLogs() : base() { }
        public OperationResultWithLogs(string? statusMessage) : this(1, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string? statusMessage) : base(statusCode, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string? statusMessage, T data) : base(statusCode, statusMessage, data) { }
        public OperationResultWithLogs(IOperationResult op) : base(op)
        {
            if (op is ILogs iLogsOp)
            {
                Logs = iLogsOp.Logs;
            }
        }
        public OperationResultWithLogs(IOperationResult op, T data) : base(op, data)
        {
            if (op is ILogs iLogsOp)
            {
                Logs = iLogsOp.Logs;
            }
        }
        public OperationResultWithLogs(int statusCode, string? statusMessage, T data, IEnumerable<string> logs) : this(statusCode, statusMessage, data)
        {
            if (logs != null)
            {
                if (logs is List<string> logsList)
                {
                    Logs = logsList;
                }
                else
                {
                    Logs = new List<string>(logs);
                }
            }
        }
        public OperationResultWithLogs(IOperationResult op, T data, IEnumerable<string> logs) : this(op, data)
        {
            if (logs != null)
            {
                if (logs is List<string> logsList)
                {
                    Logs = logsList;
                }
                else
                {
                    Logs = new List<string>(logs);
                }
            }
        }
        public OperationResultWithLogs(IOperationResultWithLogs op, T data)
        {
            Logs = op?.Logs ?? new List<string>();
            StatusCode = op?.StatusCode ?? 0;
            StatusMessage = op?.StatusMessage;
            Data = data;
        }
        public OperationResultWithLogs(ILogs logs) : this()
        {
            Logs = logs?.Logs ?? new List<string>();
            if (logs != null && logs is IOperationResult op)
            {
                StatusCode = op.StatusCode;
                StatusMessage = op.StatusMessage;
            }
        }
        public OperationResultWithLogs(ILogs logs, T data) : this(logs)
        {
            Logs = logs?.Logs ?? new List<string>();
            if (logs != null && logs is IOperationResult op)
            {
                StatusCode = op.StatusCode;
                StatusMessage = op.StatusMessage;
            }
            Data = data;
        }
        public override string ToString()
        {
            StringBuilder theReturn = new StringBuilder();
            foreach (var currentLog in Logs)
            {
                theReturn.AppendLine(currentLog);
            }
            theReturn.Append(StatusCode);
            theReturn.Append(" ");
            theReturn.Append(StatusMessage);
            return theReturn.ToString();
        }
    }
}
