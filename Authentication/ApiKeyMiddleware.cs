namespace APIAuth.Authentication
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        { 
            _next = next;
            _configuration = configuration;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Headers.TryGetValue(AuthConstants.APIKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key missing");
                return;
            }
            //Check against the appsettings config value for api key
            //Check API Key against database value here which we will use an XML file as a database source
            var apiKey = _configuration.GetValue<string>(AuthConstants.APIKeySectionName);
            if (apiKey is not null)
            {
                if (!apiKey.Equals(extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    var msg = _configuration.GetValue<string>(AuthConstants.InvalidApiMsg);
                    await context.Response.WriteAsync(msg!);
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("No Api key supplied");
                return;
            }
            await _next(context);
        }
    }
}
