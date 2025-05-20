using System.Net;
using Newtonsoft.Json;
using Appointment_System.Domain.Responses;
using Appointment_System.Domain.Enums;
using ValidationException = FluentValidation.ValidationException;
using Appointment_System.Presentation.Resources;
using Microsoft.Extensions.Localization;
using Appointment_System.Application.Localization;


public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly ILocalizationService _localizer;


    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, 
        IWebHostEnvironment env, ILocalizationService localizer)
    {
        _logger = logger;
        _env = env;
        _localizer = localizer;
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

            var errorMessages = string.Join(" | ", validationException.Errors.Select(e => e.ErrorMessage));

            var res = new ResponseBase<object>(
                StatusCodes.Status400BadRequest,
                _localizer["ValidationFailed"],
                ResponseStatus.ERROR.ToString(),
                errorMessages // Just the custom/localized messages
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
            _env.IsDevelopment() ? ex.Message : _localizer["UnexpectedError"], 
            ResponseStatus.ERROR.ToString(),
            _env.IsDevelopment() ? ex.InnerException?.Message : null
        );

        var json = JsonConvert.SerializeObject(response);
        return context.Response.WriteAsync(json);
    }
}

