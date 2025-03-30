
namespace BookVerseApp.Application.Models;

public record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message
)
{
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        => new(true, data, message);

    public static ApiResponse<T> FailResponse(string message)
        => new(false, default, message);
}
