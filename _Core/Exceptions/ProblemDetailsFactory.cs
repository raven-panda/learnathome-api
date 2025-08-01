using System.Text.Json;

namespace LearnAtHomeApi._Core.Exceptions;

public static class RpProblemDetailsFactory
{
    public static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        string type = "about:blank"
    )
    {
        var problem = new
        {
            type,
            title,
            status = statusCode,
            detail,
            instance = context.Request.Path,
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json; charset=utf-8";
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
