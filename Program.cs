using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using ExpenseTracker.Common.Extensions;
using ExpenseTracker.Common.Middlewares;
using ExpenseTracker.Common.Helpers;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(new JsonFormatter(), "Logs/log.json", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // Seq integration
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                Console.WriteLine("Token received: " + context.Token);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT Authentication Failed: " + context.Exception);
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + context.Exception.InnerException.Message);
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "You are not authorized. Please login first."
                });
                return context.Response.WriteAsync(json);
            }
        };
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        if (builder.Environment.IsDevelopment())
        {
            tokenValidationParameters.ValidateIssuer = false;
            tokenValidationParameters.ValidateAudience = false;
        }
        else
        {
            tokenValidationParameters.ValidateIssuer = true;
            tokenValidationParameters.ValidateAudience = true;
        }
        options.TokenValidationParameters = tokenValidationParameters;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:8080")
            //.WithOrigins("http://expensetracker-webapi.s3-website-ap-southeast-2.amazonaws.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            // .AllowCredentials() // Uncomment if needed
    );
});

builder.Services.AddAuthorization();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Always use ApiKey for Swagger UI preauthorization
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey // This is required for preauthorizeApiKey to work
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseSwagger();
app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.InjectJavascript("/swagger/custom.js");
    });
}
else
{
    // Require authentication for Swagger UI in production
    app.UseWhen(context => context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
    {
        appBuilder.UseAuthentication();
        appBuilder.UseAuthorization();
        appBuilder.Use(async (context, next) =>
        {
            var user = context.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized - Please login first");
                return;
            }

            // check role claim
            if (!user.IsInRole("Admin"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden - Admin role required");
                return;
            }

            await next();
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseCors("AllowFrontend");
// app.UseHttpsRedirection(); // Uncomment if using HTTPS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Async database seeding
try
{
    await DbSeeder.SeedDatabaseAsync(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"Seeding failed: {ex.Message}");
}

app.MapGet("/health", () => Results.Ok("OK"));
app.Run();
