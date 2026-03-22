using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Mặc định là lỗi 500
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "Đã có lỗi hệ thống xảy ra. Vui lòng thử lại sau.";

        // Tùy biến mã lỗi dựa trên loại Exception (Nghiệp vụ)
        if (exception is UnauthorizedAccessException) statusCode = (int)HttpStatusCode.Unauthorized;
        else if (exception is KeyNotFoundException) statusCode = (int)HttpStatusCode.NotFound;
        // Bạn có thể thêm các Custom Exception của riêng mình ở đây

        context.Response.StatusCode = statusCode;

        var response = new ErrorDetails
        {
            StatusCode = statusCode,
            Message = exception.Message,
            // Chỉ hiện chi tiết lỗi nếu đang code (Development)
            Details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null
        };

        await context.Response.WriteAsync(response.ToString());
    }
}