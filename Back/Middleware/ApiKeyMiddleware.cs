using System.Net;

namespace Back.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER_NAME = "X-API-KEY";
        private const string UNAUTHORIZED_RESPONSE = "API Key was not provided.";
        private const string FORBIDDEN_RESPONSE = "Unauthorized client.";
        private const string API_KEY_CONFIG = "ApiKey";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(UNAUTHORIZED_RESPONSE);
                return;
            }

            var apiKey = configuration.GetValue<string>(API_KEY_CONFIG);

            if (apiKey == null || !apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync(FORBIDDEN_RESPONSE);
                return;
            }

            await _next(context);
        }
    }
}
