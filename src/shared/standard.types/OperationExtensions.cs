using System;
using System.Threading.Tasks;

namespace standard.types
{
    public static class OperationExtensions
    {
        public static async Task<IOperationResult> Pipe(this Task<IOperationResult> operation, Func<Task<IOperationResult>> func)
        {
            var ret = await operation.ConfigureAwait(false);
            if (ret.Fail()) return ret;

            return await func().ConfigureAwait(false);
        }

        public static async Task<IOperationResult> Pipe(this IOperationResult operation, Func<Task<IOperationResult>> func)
        {
            if (operation.Fail()) return operation;

            return await func().ConfigureAwait(false);
        }

        public static IOperationResult Pipe(this IOperationResult operation, Func<IOperationResult> func)
        {
            if (operation.Fail()) return operation;

            return func();
        }

        public static bool IsSuccess(this IOperationResult result)
        {
            return result == null || result.StatusCode == 0;
        }

        public static bool Success(this IOperationResult result) => IsSuccess(result);

        public static OperationResult<TIn> ToOperationResult<TIn>(this TIn data)
        {
            return new OperationResult<TIn>() { Data = data };
        }

        public static TOp ToFailingOperationResult<TOp>(this Exception ex)
            where TOp : IOperationResult, new()
        {
            return new TOp()
            {
                StatusCode = 1,
                StatusMessage = ex.ToString()
            };
        }
    }
}