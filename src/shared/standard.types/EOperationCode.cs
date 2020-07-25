namespace standard.types
{
    public enum EOperationCode
    {
        Ok = 0,
        Error = 1,
        UnhandledError = 2,
        ThrottlerReject = 3,
        NoContent = 204,
        NotModified = 304,
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        TooManyRequests = 429,
        BadGateway = 502,
        GatewayTimeout = 504,

        /// <summary>
        ///     The operation was cancelled (typically by the caller).
        /// </summary>
        RpcExceptionCancelled = 1000 + 1,

        /// <summary>
        ///     Unknown error. An example of where this error may be returned is if a Status
        ///     value received from another address space belongs to an error-space that is not
        ///     known in this address space. Also errors raised by APIs that do not return enough
        ///     error information may be converted to this error.
        /// </summary>
        RpcExceptionUnknown = 1000 + 2,

        /// <summary>
        ///     Client specified an invalid argument. Note that this differs from FAILED_PRECONDITION.
        ///     INVALID_ARGUMENT indicates arguments that are problematic regardless of the state
        ///     of the system (e.g., a malformed file name).
        /// </summary>
        RpcExceptionInvalidArgument = 1000 + 3,

        /// <summary>
        ///     Deadline expired before operation could complete. For operations that change
        ///     the state of the system, this error may be returned even if the operation has
        ///     completed successfully. For example, a successful response from a server could
        ///     have been delayed long enough for the deadline to expire.
        /// </summary>
        RpcExceptionDeadlineExceeded = 1000 + 4,

        /// <summary>
        ///     Some requested entity (e.g., file or directory) was not found.
        /// </summary>
        RpcExceptionNotFound = 1000 + 5,

        /// <summary>
        ///     Some entity that we attempted to create (e.g., file or directory) already exists.
        /// </summary>
        RpcExceptionAlreadyExists = 1000 + 6,

        /// <summary>
        ///     The caller does not have permission to execute the specified operation. PERMISSION_DENIED
        ///     must not be used for rejections caused by exhausting some resource (use RESOURCE_EXHAUSTED
        ///     instead for those errors). PERMISSION_DENIED must not be used if the caller can
        ///     not be identified (use UNAUTHENTICATED instead for those errors).
        /// </summary>
        RpcExceptionPermissionDenied = 1000 + 7,

        /// <summary>
        ///     Some resource has been exhausted, perhaps a per-user quota, or perhaps the entire
        ///     file system is out of space.
        /// </summary>
        RpcExceptionResourceExhausted = 1000 + 8,

        /// <summary>
        ///     Operation was rejected because the system is not in a state required for the
        ///     operation's execution. For example, directory to be deleted may be non-empty,
        ///     an rmdir operation is applied to a non-directory, etc.
        /// </summary>
        RpcExceptionFailedPrecondition = 1000 + 9,

        /// <summary>
        ///     The operation was aborted, typically due to a concurrency issue like sequencer
        ///     check failures, transaction aborts, etc.
        /// </summary>
        RpcExceptionAborted = 1000 + 10,

        /// <summary>
        ///     Operation was attempted past the valid range. E.g., seeking or reading past end
        ///     of file.
        /// </summary>
        RpcExceptionOutOfRange = 1000 + 11,

        /// <summary>
        ///     Operation is not implemented or not supported/enabled in this service.
        /// </summary>
        RpcExceptionUnimplemented = 1000 + 12,

        /// <summary>
        ///     Internal errors. Means some invariants expected by underlying system has been
        ///     broken. If you see one of these errors, something is very broken.
        /// </summary>
        RpcExceptionInternal = 1000 + 13,

        /// <summary>
        ///     The service is currently unavailable. This is a most likely a transient condition
        ///     and may be corrected by retrying with a backoff. Note that it is not always safe
        ///     to retry non-idempotent operations.
        /// </summary>
        RpcExceptionUnavailable = 1000 + 14,

        /// <summary>
        ///     Unrecoverable data loss or corruption.
        /// </summary>
        RpcExceptionDataLoss = 1000 + 15,

        /// <summary>
        ///     The request does not have valid authentication credentials for the operation.
        /// </summary>
        RpcExceptionUnauthenticated = 1000 + 16
    }
}