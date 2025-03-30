using BookVerseApp.Application.Models;

namespace BookVerseApp.Application.Helpers;

public static class ApiExecutor
{
    public static async Task<ApiResponse<T>> Execute<T>(
     Func<Task<T>> operation,
     ILogger logger,
     string operationName = "Operation",
     bool logErrors = true,
     string? nullMessage = "Requested resource not found.")
    {
        try
        {
            var result = await operation();

            if (result == null)
            {
                if (logErrors)
                    logger.LogWarning($"[ApiExecutor] Null result in: {operationName}");

                return ApiResponse<T>.FailResponse(nullMessage ?? "Not found");
            }

            return ApiResponse<T>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            if (logErrors)
                logger.LogError(ex, $"[ApiExecutor] Error in: {operationName}");

            return ApiResponse<T>.FailResponse("Something went wrong. Please try again later.");
        }
    }

}
