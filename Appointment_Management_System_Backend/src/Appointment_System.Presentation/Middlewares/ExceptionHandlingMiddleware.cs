using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Appointment_System.Domain.Responses;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Appointment_System.Domain.Enums;
using System.Net.Http;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using ValidationException = FluentValidation.ValidationException;


public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    // Logs and handles unhandled exceptions globally.
    // Logs the error via Serilog for developer diagnostics.
    // Responds with safe structured JSON message to clients.
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleException(context, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var res = new ResponseBase<object>(
                StatusCodes.Status400BadRequest,
                "Validation failed.",
                ResponseStatus.ERROR.ToString(),
                validationException.Message // This will be a validation failure message
            );

            var json2 = JsonConvert.SerializeObject(res);
            return context.Response.WriteAsync(json2);
        }

        var statusCode = ex switch
        {
            ArgumentException or ArgumentNullException or ArgumentOutOfRangeException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;
        //context.Response.ContentType = "application/json";

        var response = new ResponseBase<object>(
            (int)statusCode,
            _env.IsDevelopment() ? ex.Message : "Something went wrong.",
            ResponseStatus.ERROR.ToString(),
            _env.IsDevelopment() ? ex.InnerException?.Message : null
        );

        var json = JsonConvert.SerializeObject(response);
        return context.Response.WriteAsync(json);
    }
}

