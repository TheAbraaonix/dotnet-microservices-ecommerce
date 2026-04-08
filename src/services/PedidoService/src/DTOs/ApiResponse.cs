namespace PedidoService.DTOs;

public enum ErrorCode
{
    None = 0,
    ValidationError = 1,
    NotFound = 2,
    Conflict = 3,
    InternalError = 4
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ErrorCode? ErrorCode { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            ErrorCode = null,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> ErrorResponse(
        string message, 
        ErrorCode errorCode,
        int statusCode = 400, 
        List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            StatusCode = statusCode,
            Errors = errors
        };
    }
}
