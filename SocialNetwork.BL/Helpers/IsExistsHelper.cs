using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;

namespace SocialNetwork.BL.Helpers;

public static class IsExistsHelper
{
    public static void IsExists(this ILogger logger, Object? model, CustomException exceptionType)
    {
        if (model is not null) return;
        logger.LogError(exceptionType.Message);
        throw exceptionType;
    }
}