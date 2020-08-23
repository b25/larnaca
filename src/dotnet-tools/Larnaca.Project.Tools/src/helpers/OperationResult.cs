using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
/// <summary>
/// avoid referencing framework library
/// </summary>
namespace Larnaca.Project.Tools
{
    internal interface IOperationResult
    {
        int StatusCode { get; set; }
        string StatusMessage { get; set; }
    }
    internal interface ILogs
    {
        List<string> Logs { get; set; }
    }
    internal interface IOperationResultWithLogs : IOperationResult, ILogs { }
    internal interface IOperationResult<T> : IOperationResult
    {
        T Data { get; set; }
    }
    internal interface IOperationResultWithLogs<T> : IOperationResult<T>, ILogs { }
    internal class OperationResult : IOperationResult
    {
        public OperationResult() { }
        public OperationResult(string statusMessage) : this(1, statusMessage) { }
        public OperationResult(int statusCode, string statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }
        public OperationResult(IOperationResult op) : this(op?.StatusCode ?? 0, op?.StatusMessage) { }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public override string ToString()
        {
            return StatusCode.ToString() + " " + StatusMessage;
        }
    }
    internal class OperationResultWithLogs : OperationResult, IOperationResultWithLogs
    {
        public List<string> Logs { get; set; } = new List<string>();
        public OperationResultWithLogs() : base() { }
        public OperationResultWithLogs(string statusMessage) : this(1, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string statusMessage) : base(statusCode, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string statusMessage, IEnumerable<string> logs) : this(statusCode, statusMessage)
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
    internal class OperationResult<T> : OperationResult, IOperationResult<T>
    {
        public OperationResult() : base() { }
        public OperationResult(string statusMessage) : this(1, statusMessage) { }
        public OperationResult(int statusCode, string statusMessage) : base(statusCode, statusMessage) { }
        public OperationResult(string statusMessage, T data) : this(1, statusMessage, data) { }
        public OperationResult(int statusCode, string statusMessage, T data) : this(statusCode, statusMessage)
        {
            Data = data;
        }
        public OperationResult(IOperationResult op) : this(op?.StatusCode ?? 0, op?.StatusMessage) { }
        public OperationResult(IOperationResult op, T data) : this(op?.StatusCode ?? 0, op?.StatusMessage)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
    internal class OperationResultWithLogs<T> : OperationResult<T>, IOperationResultWithLogs<T>
    {
        public List<string> Logs { get; set; } = new List<string>();
        public OperationResultWithLogs() : base() { }
        public OperationResultWithLogs(string statusMessage) : this(1, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string statusMessage) : base(statusCode, statusMessage) { }
        public OperationResultWithLogs(int statusCode, string statusMessage, T data) : base(statusCode, statusMessage, data) { }
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
        public OperationResultWithLogs(int statusCode, string statusMessage, T data, IEnumerable<string> logs) : this(statusCode, statusMessage, data)
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
    internal static class OperationResult_Extensions
    {
        public static OperationResult<T> ToOperationResult<T>(this T data) => new OperationResult<T>((OperationResult)null, data);
        public static OperationResultWithLogs<T> ToOperationResultWithLogs<T>(this T data) => new OperationResultWithLogs<T>(null, data);
        public static bool SelfOrNestedResultFail<T>(this IOperationResult<T> op, out IOperationResult topFailingResult)
            where T : IOperationResult => SelfOrNestedResultFailGeneric<IOperationResult<T>, T>(op, out topFailingResult);
        private static bool SelfOrNestedResultFailGeneric<TOp, T>(TOp op, out IOperationResult topFailingResult)
            where T : IOperationResult
            where TOp : IOperationResult<T>
        {
            if (op == null)
            {
                topFailingResult = null;
                return false;
            }
            if (op.Fail())
            {
                topFailingResult = op;
                return true;
            }
            if (op.Data == null)
            {
                topFailingResult = null;
                return false;
            }
            if (op.Data.Fail())
            {
                topFailingResult = op.Data;
                return true;
            }

            var dataType = op.Data.GetType();

            if (dataType.IsConstructedGenericType)
            {
                if (dataType.GetInterfaces().Any(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == typeof(IOperationResult<>)))
                {
                    var genericType = dataType.GetGenericArguments().First();
                    if (genericType.GetInterfaces().Contains(typeof(IOperationResult)))
                    {
                        var method = typeof(OperationResult_Extensions).GetMethod(nameof(SelfOrNestedResultFailGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        var methodParams = new object[] { op.Data, null };
                        method = method.MakeGenericMethod(new Type[] { dataType, genericType });
                        var result = method.Invoke(null, methodParams);
                        topFailingResult = (IOperationResult)methodParams[1];
                        return (bool)result;
                    }
                }
            }
            topFailingResult = null;
            return false;
        }
        public static bool Fail(this IOperationResult op) => (op?.StatusCode ?? 0) != 0;
        public static bool IsFail(this IOperationResult op) => op.Fail();
        public static bool Success(this IOperationResult op) => !op.Fail();
        public static bool IsSuccess(this IOperationResult op) => op.Success();
    }
}
