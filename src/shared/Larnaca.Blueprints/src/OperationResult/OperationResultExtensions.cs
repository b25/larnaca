using System;
using System.Linq;

namespace Larnaca.Blueprints
{
    public static class OperationResult_Extensions
    {
        public static OperationResult<T> ToOperationResult<T>(this T data) => new OperationResult<T>(0, null, data);
        public static OperationResultWithLogs<T> ToOperationResultWithLogs<T>(this T data) => new OperationResultWithLogs<T>(0, null, data);
        public static bool SelfOrNestedResultFail<T>(this IOperationResult<T> op, out IOperationResult? topFailingResult)
            where T : IOperationResult => SelfOrNestedResultFailGeneric<IOperationResult<T>, T>(op, out topFailingResult);
        private static bool SelfOrNestedResultFailGeneric<TOp, T>(TOp op, out IOperationResult? topFailingResult)
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
                        var methodParams = new object?[] { op.Data, null };
                        method = method.MakeGenericMethod(new Type[] { dataType, genericType });
                        var result = method.Invoke(null, methodParams);
                        topFailingResult = (IOperationResult?)methodParams[1];
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