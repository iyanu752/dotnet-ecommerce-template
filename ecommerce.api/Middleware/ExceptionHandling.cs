using System;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace ecommerce.api;

public class ExceptionHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandling> _logger;
    private readonly IWebHostEnvironment _enviroment;

    public ExceptionHandling(
        RequestDelegate next, 
        ILogger<ExceptionHandling> logger, 
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _enviroment = environment;
    }

    public async Task InvokeAsync( HttpContext context )
    {
        try
        {
            await _next(context);

        }catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occoured");
        context.Response.ContentType = "application/json";
        var statusCode = exception switch
        {
            KeyNotFoundException => HttpStatusCode.NotFound,
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new ApiErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = statusCode switch
            {
                HttpStatusCode.NotFound => "The required recourse was not found",
                HttpStatusCode.BadRequest => "The request is invalid" ,
                HttpStatusCode.Unauthorized => "You are not authourized to perform this action",
                _=> "An unexpected error occoured"  
            },
            Details = _enviroment.IsDevelopment()? exception.Message : null
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
