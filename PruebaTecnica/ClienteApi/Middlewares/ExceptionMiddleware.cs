using System.Text.Json;

namespace ClienteApi.Middlewares
{
    public class ExceptionMiddleware
    {
        #region Vars
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        #endregion

        #region Constructors
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        #endregion

        #region Methods
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Manages if any error occurred in ANY LAYER of the app
                _logger.LogError(ex, "Algo salió mal: {Message}", ex.Message);
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var response = new {
                StatusCode = 500,
                Message = "Error interno del servidor. Inténtelo más tarde."
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        #endregion
    }
}