using System;
using System.Runtime.Serialization;

namespace standard.types
{
    [DataContract]
    public class OperationResult : IOperationResult
    {
        public OperationResult()
        {
            StatusCode = (int)EOperationCode.Ok;
        }

        public OperationResult(Exception ex)
        {
            StatusCode = (int)EOperationCode.Error;
            StatusMessage = ex.ToString();
        }

        public OperationResult(string statusMessage)
        {
            StatusCode = (int)EOperationCode.Error;
            StatusMessage = statusMessage;
        }

        public OperationResult(int statusCode, string statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }

        public OperationResult(EOperationCode statusCode, string statusMessage)
        {
            StatusCode = (int)statusCode;
            StatusMessage = statusMessage;
        }

        public OperationResult(IOperationResult operationResult)
        {
            StatusCode = operationResult.StatusCode;
            StatusMessage = operationResult.StatusMessage;
        }

        [DataMember(Order = 1)]
        public int StatusCode { get; set; }

        [DataMember(Order = 2)]
        public string StatusMessage { get; set; }

        public override string ToString()
        {
            return $"Code: {StatusCode} Message:'{StatusMessage}'";
        }

        public bool IsSuccess()
        {
            return StatusCode == (int)EOperationCode.Ok;
        }

        public bool Success() => IsSuccess();

        public bool IsFail() => !IsSuccess();

        public bool Fail() => IsFail();
    }

    [DataContract]
    public class OperationResult<T> : IOperationResult<T>
    {
        public OperationResult()
        {
            StatusCode = (int)EOperationCode.Ok;
        }

        public OperationResult(string statusMessage)
        {
            StatusCode = (int)EOperationCode.Error;
            StatusMessage = statusMessage;
            Data = default(T);
        }

        public OperationResult(T data)
        {
            StatusCode = (int)EOperationCode.Ok;

            Data = data;
        }

        public OperationResult(int statusCode, string statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            Data = default(T);
        }

        public OperationResult(int statusCode, string statusMessage, T data)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            Data = data;
        }

        public OperationResult(T data, int statusCode, string statusMessage = null)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            Data = data;
        }

        public OperationResult(IOperationResult operationResult)
        {
            StatusCode = operationResult.StatusCode;
            StatusMessage = operationResult.StatusMessage;
            Data = default(T);
        }

        public OperationResult(IOperationResult<T> operationResult)
        {
            StatusCode = operationResult.StatusCode;
            StatusMessage = operationResult.StatusMessage;
            Data = operationResult.Data;
        }

        [DataMember(Order = 1)]
        public int StatusCode { get; set; }

        [DataMember(Order = 2)]
        public string StatusMessage { get; set; }

        [DataMember(Order = 3)]
        public T Data { get; set; }

        public override string ToString()
        {
            return $"Code: {StatusCode} Message:'{StatusMessage}'";
        }

        public bool IsSuccess()
        {
            return StatusCode == (int)EOperationCode.Ok;
        }

        public bool Success() => IsSuccess();

        public bool IsFail() => !IsSuccess();

        public bool Fail() => IsFail();
    }
}