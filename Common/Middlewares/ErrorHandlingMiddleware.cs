using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using System.Data.Common;

namespace ExpenseTracker.Common.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Go to next middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");

                int statusCode = StatusCodes.Status500InternalServerError;
                string message = "Internal server error";

                // Map common exception types to appropriate status codes
                switch (ex)
                {
                    case ArgumentException _:
                    case ValidationException _:
                        statusCode = StatusCodes.Status400BadRequest;
                        message = ex.Message;
                        break;
                    case UnauthorizedAccessException _:
                        statusCode = StatusCodes.Status401Unauthorized;
                        message = "Unauthorized";
                        break;
                    case NotFoundException _:
                        statusCode = StatusCodes.Status404NotFound;
                        message = ex.Message;
                        break;
                    case ConflictException _:
                        statusCode = StatusCodes.Status409Conflict;
                        message = ex.Message;
                        break;
                    case KeyNotFoundException _:
                        statusCode = StatusCodes.Status404NotFound;
                        message = ex.Message;
                        break;
                    case DbException _:
                        statusCode = StatusCodes.Status500InternalServerError;
                        message = "A database error occurred.";
                        break;
                    default:
                        // keep generic message for production
                        if (_env.IsDevelopment())
                        {
                            message = ex.Message;
                        }
                        break;
                }

                var response = ApiResponse<object>.Fail(message);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
