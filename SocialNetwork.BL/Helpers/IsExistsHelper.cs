using Microsoft.Extensions.Logging;
using SocialNetwork.BL.Exceptions;

namespace SocialNetwork.BL.Helpers;

public static class IsExistsHelper
{
    private static readonly ILogger _logger;
    
    public static void IsExists(Object? model, CustomException exceptionType)
    {
        if (model is not null) return;
        _logger.LogError(exceptionType.Message);
        throw exceptionType;
    }
}