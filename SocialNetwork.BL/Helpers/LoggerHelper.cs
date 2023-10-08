using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;

namespace SocialNetwork.BL.Helpers;

public static class LoggerHelper
{
    public static void LogAndThrowErrorIfNull(this ILogger logger, Object? model, CustomException exceptionType)
    {
        if (model is not null) return;
        logger.LogError(exceptionType.Message);
        throw exceptionType;
    }
    public static void LogAndThrowErrorIfNull(this ILogger logger, List<Object>? model, CustomException exceptionType)
    {

        foreach (var m in model)
        {
            if (m is null)
            {
                logger.LogError(exceptionType.Message);
                throw exceptionType;
            }
        }
    }
}