using Grpc.Core.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace envoy.controller.Cache
{
    /// <summary>
    /// Extensions providing using formatable strings as messages.
    /// It can be used to prevent string allocations for desabled log levels.
    /// </summary>
    public static class LoggerExtensions
    {
        public static void DebugF(this ILogger logger, FormattableString formatableMessage)
        {
            logger.Debug(formatableMessage.Format, formatableMessage.GetArguments());
        }

        public static void InfoF(this ILogger logger, FormattableString formatableMessage)
        {
            logger.Debug(formatableMessage.Format, formatableMessage.GetArguments());
        }

        public static void WarningF(this ILogger logger, FormattableString formatableMessage)
        {
            logger.Debug(formatableMessage.Format, formatableMessage.GetArguments());
        }

        public static void ErrorF(this ILogger logger, FormattableString formatableMessage)
        {
            logger.Debug(formatableMessage.Format, formatableMessage.GetArguments());
        }
    }
}
