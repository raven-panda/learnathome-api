using System.Net;
using System.Text.Json;
using LearnAtHomeApi._Core.Exceptions.Entity;

namespace LearnAtHomeApi._Core.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context); // passe au middleware suivant
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = GetStatusCode(ex);

            var response = new
            {
                status = context.Response.StatusCode,
                message = ex.Message,
                error = ex.GetType().Name
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    private static int GetStatusCode(Exception ex)
    {
        return ex switch
        {
            EntityNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int) HttpStatusCode.InternalServerError
        };
    }
}