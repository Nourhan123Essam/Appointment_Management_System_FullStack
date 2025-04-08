using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Appointment_System.Domain.Responses;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Appointment_System.Domain.Enums;

public class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleException(httpContext, ex);
        }
    }

    private Task HandleException(HttpContext context, Exception ex)
    {
        var statusCode = HttpStatusCode.InternalServerError;

        if (ex is ArgumentNullException)
            statusCode = HttpStatusCode.BadRequest;
        else if (ex is ArgumentException)
            statusCode = HttpStatusCode.BadRequest;
        else if (ex is KeyNotFoundException)
            statusCode = HttpStatusCode.NotFound;
        else if (ex is ArgumentOutOfRangeException)
            statusCode = HttpStatusCode.BadRequest; 

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ResponseBase<object>(
            (int)statusCode,
            ex.Message,
            ResponseStatus.ERROR.ToString(),
            ex.InnerException?.Message
        );

        var json = JsonConvert.SerializeObject(response);
        return context.Response.WriteAsync(json);
    }


}
