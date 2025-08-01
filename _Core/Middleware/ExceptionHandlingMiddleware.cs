using LearnAtHomeApi._Core.Exceptions;
using LearnAtHomeApi._Core.Exceptions.Entity;

namespace LearnAtHomeApi._Core.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env, IConfiguration config)
{
    private readonly string _baseUrl = env.IsDevelopment() ? config["ASPNETCORE_URLS"]?.Split(';')[0] ?? "http://localhost:5146" : "https://learnathome.io";
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context); // passe au middleware suivant
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            var (status, title, type) = GetStatus(ex);

            await RpProblemDetailsFactory.WriteProblemDetails(
                context,
                status,
                title,
                ex.Message,
                type
            );
        }
    }

    private (int status, string title, string type) GetStatus(Exception ex)
    {
        return ex switch
        {
            EntityNotFoundException => (404, "Entity Not Found", $"{_baseUrl}/docs/errors/entity-not-found.html"),
            _ => (500, "Internal Server Error", "https://tools.ietf.org/html/rfc9110#section-15.6.1")
        };
    }
}