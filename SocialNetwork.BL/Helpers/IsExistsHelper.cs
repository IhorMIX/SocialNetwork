using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;

namespace SocialNetwork.BL.Helpers;

public static class IsExistsHelper
{
    public static void IsExists(Object? model, CustomException exceptionType, ILogger logger)
    {
        if (model is not null) return;
        logger.LogError(exceptionType.Message);
        throw exceptionType;
    }
}