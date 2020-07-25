using System;

namespace standard.types
{
    public interface ITimedOperationResult : IOperationResult
    {
        TimeSpan Duration { get; set; }
    }
}