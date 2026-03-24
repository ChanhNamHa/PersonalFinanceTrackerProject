using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PersonalFinanceTracker.Application.Exceptions;
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

        if (exception is UnauthorizedAccessException) statusCode = (int)HttpStatusCode.Unauthorized;
        else if (exception is BudgetExceededException)
        {
            statusCode = (int)HttpStatusCode.BadRequest; // 400
            message = exception.Message;
        }
        else if (exception is KeyNotFoundException) statusCode = (int)HttpStatusCode.NotFound;

        context.Response.StatusCode = statusCode;

        var response = new ErrorDetails
        {
            StatusCode = statusCode,
            Message = exception.Message,
            Details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null
        };

        await context.Response.WriteAsync(response.ToString());
    }
}