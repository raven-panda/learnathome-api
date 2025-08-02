using LearnAtHomeApi;
using LearnAtHomeApi._Core.Middleware;
using LearnAtHomeApi.Authentication.Service;
using LearnAtHomeApi.StudentTask.Repository;
using LearnAtHomeApi.StudentTask.Service;
using LearnAtHomeApi.User.Repository;
using LearnAtHomeApi.User.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=database.db")
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRpUserService, RpUserService>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStudentTaskRepository, StudentTaskRepositoryImp>();
builder.Services.AddScoped<IStudentTaskService, StudentTaskService>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("JwtSettings", options)
    )
    .AddCookie(
        CookieAuthenticationDefaults.AuthenticationScheme,
        options => builder.Configuration.Bind("CookieSettings", options)
    );

var app = builder.Build();
app.UsePathBase("/api/v1");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();
