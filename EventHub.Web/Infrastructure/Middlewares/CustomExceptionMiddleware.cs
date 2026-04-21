namespace EventHub.Web.Infrastructure.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware. Path: {Path}", context.Request.Path);
                // redirect to the generic error page instead of crashing
                context.Response.Redirect("/Home/Error");
            }
        }
    }

    // extension method so we can call app.UseCustomExceptionMiddleware() in Program.cs
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder app)
            => app.UseMiddleware<CustomExceptionMiddleware>();
    }
}
