using System.Net;
using LearnAtHomeApi._Core.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace LearnAtHomeApi._Core.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IWebHostEnvironment env,
    IConfiguration config
)
{
    private readonly string _baseUrl = env.IsDevelopment()
        ? config["ASPNETCORE_URLS"]?.Split(';')[0] ?? "http://localhost:5146"
        : "https://learnathome.io";

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
            EntityNotFoundException => (
                (int)HttpStatusCode.NotFound,
                "Entity Not Found",
                $"{_baseUrl}/docs/errors/entity-not-found.html"
            ),
            EntityUniqueConstraintViolationException => (
                (int)HttpStatusCode.Conflict,
                "Entity Unique Constraint Violation",
                $"{_baseUrl}/docs/errors/entity-unique-constraint-violation.html"
            ),
            PasswordsNotMatchingException => (
                (int)HttpStatusCode.BadRequest,
                "Passwords Not Matching",
                $"{_baseUrl}/docs/errors/passwords-not-matching.html"
            ),
            SecurityTokenException => (
                (int)HttpStatusCode.Unauthorized,
                "Security Token Invalid",
                $"{_baseUrl}/docs/errors/security-token-invalid.html"
            ),
            InvalidCredentialsException => (
                (int)HttpStatusCode.BadRequest,
                "Invalid Credentials",
                $"{_baseUrl}/docs/errors/invalid-credentials.html"
            ),
            NotImplementedException => (
                (int)HttpStatusCode.NotImplemented,
                "Not Implemented",
                "https://tools.ietf.org/html/rfc9110#section-15.6.2"
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error",
                "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            ),
        };
    }
}
