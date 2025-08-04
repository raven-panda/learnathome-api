using System.Text;
using LearnAtHomeApi;
using LearnAtHomeApi._Core.Middleware;
using LearnAtHomeApi.Authentication.Security;
using LearnAtHomeApi.Authentication.Service;
using LearnAtHomeApi.StudentTask.Repository;
using LearnAtHomeApi.StudentTask.Service;
using LearnAtHomeApi.User.Repository;
using LearnAtHomeApi.User.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

const string corsPolicyName = "AllowOrigins";

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration["Jwt:Secret"] == null)
    throw new ArgumentNullException(
        nameof(builder.Configuration),
        "Jwt:Secret configuration is null"
    );

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=database.db")
    );
}

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRpUserService, RpUserService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<IStudentTaskRepository, StudentTaskRepositoryImp>();
builder.Services.AddScoped<IStudentTaskService, StudentTaskService>();

builder.Services.AddSingleton<TokenProvider>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: corsPolicyName,
        policy =>
        {
            policy
                .WithOrigins("https://localhost:*", "http://localhost:*")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (
                    ctx.Request.Cookies.ContainsKey(
                        builder.Configuration["Jwt:SessionTokenCookieName"]!
                    )
                )
                    ctx.Token = ctx.Request.Cookies[
                        builder.Configuration["Jwt:SessionTokenCookieName"]!
                    ];

                return Task.CompletedTask;
            },
        };
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UsePathBase("/api/v1");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!builder.Environment.IsEnvironment("Testing"))
    {
        db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors(corsPolicyName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
