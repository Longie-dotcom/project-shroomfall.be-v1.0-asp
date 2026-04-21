using System.Text.Json;
using Application.ApplicationException;

namespace API.Middleware
{
    public class GlobalExceptionHandler
    {
        #region Attributes
        private readonly RequestDelegate requestDelegate;
        #endregion

        #region Properties
        #endregion

        public GlobalExceptionHandler(RequestDelegate requestDelegate)
        {
            this.requestDelegate = requestDelegate;
        }

        #region Methods
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = StatusCodes.Status500InternalServerError;
            var type = "Internal Server Error";
            var message = "An internal error occurred. Please try again later.";
            string? details = null;

            switch (exception)
            {
                case BadRequest:
                    statusCode = StatusCodes.Status400BadRequest;
                    type = "Bad Request";
                    message = exception.Message;
                    break;

                case NotFound:
                    statusCode = StatusCodes.Status404NotFound;
                    type = "Not Found";
                    message = exception.Message;
                    break;

                default:
                    details = exception.ToString();
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new
            {
                type,
                message,
                details
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }
        #endregion
    }
}