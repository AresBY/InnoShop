using InnoShop.Users.Application.Configurations;
using InnoShop.Users.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace InnoShop.Users.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly ProblemDetailsSettings _problemDetailsSettings;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IOptions<ProblemDetailsSettings> options)
        {
            _next = next;
            _logger = logger;
            _problemDetailsSettings = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught in middleware");

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var problemDetails = new ProblemDetails
            {
                Type = _problemDetailsSettings.TypeUri,
                Detail = exception.Message
            };

            switch (exception)
            {
                case ValidationException:
                    problemDetails.Title = "Validation error";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    break;

                case NotFoundException:
                    problemDetails.Title = "Resource not found";
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Title = "Unauthorized access";
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    break;

                default:
                    problemDetails.Title = "Internal server error";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

            var json = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(json);
        }
    }
}
