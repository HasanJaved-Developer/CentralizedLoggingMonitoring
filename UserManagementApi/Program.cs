using UserManagementApi;
using UserManagementApi.Data;
using UserManagementApi.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Application", "CentralizedLogging") // change if needed
    .CreateLogger();

builder.Host.UseSerilog();

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Connection: {builder.Configuration.GetConnectionString("DefaultConnection")}");

// DB connection string (SQL Server example)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Centralized Logging API",
        Version = "v1",
        Description = "API for centralized error logging and monitoring"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true); // ok if your Swashbuckle version supports the bool

});

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    using (Serilog.Context.LogContext.PushProperty("Environment", app.Environment.EnvironmentName))
    using (Serilog.Context.LogContext.PushProperty("Service", "CoreAPI"))
    using (Serilog.Context.LogContext.PushProperty("CorrelationId", ctx.TraceIdentifier))
    {
        await next();
    }
});

// Middleware should be early in the pipeline
app.UseMiddleware<RequestAudibilityMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
