using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers for handling HTTP requests.
builder.Services.AddControllers();

// Retrieve the SQL connection string from configuration.
var sqlConnectionString = builder.Configuration.GetValue<string>("SqlConnectionString");
var sqlConnectionStringFound = !string.IsNullOrWhiteSpace(sqlConnectionString);

// Register OpenAPI/Swagger for API documentation and testing.
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MySecureBackend API",
        Version = "v1",
    });
});

builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);

// Register CORS policy for Unity/external clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnity", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register authorization services for securing endpoints.
builder.Services.AddAuthorization();

// Register ASP.NET Core Identity with Dapper stores for user authentication and management.
// Configures password and user requirements.
builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false; // Allow passwords without special characters (development only!)
    options.Password.RequireDigit = false; // Allow passwords without numbers
    options.Password.RequireUppercase = false; // Allow passwords without uppercase
    options.Password.RequireLowercase = false; // Allow passwords without lowercase
})
.AddRoles<IdentityRole>()
.AddDapperStores(options =>
{
    options.ConnectionString = sqlConnectionString;
});

// Register IHttpContextAccessor for accessing HTTP context in services (e.g., to get current user info).
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IAuthenticationService, AspNetIdentityAuthenticationService>();

// Register application repositories.
// By default, use an in-memory repository for example objects.
builder.Services.AddTransient<IExampleObjectRepository, MemoryExampleObjectRepository>();

// To use a SQL-backed repository instead, uncomment the following line:
//builder.Services.AddTransient<IExampleObjectRepository, SqlExampleObjectRepository>(o => new SqlExampleObjectRepository(sqlConnectionString!));

// Register Environment2D and Object2D repositories
builder.Services.AddTransient<IEnvironment2DRepository>(provider => new SqlEnvironment2DRepository(sqlConnectionString!));
builder.Services.AddTransient<IObject2DRepository>(provider => new SqlObject2DRepository(sqlConnectionString!));

var app = builder.Build();

// Register OpenAPI/Swagger endpoints.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MySecureBackend API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
        options.CacheLifetime = TimeSpan.Zero; // Disable caching for development

        // Inject a warning in the Swagger UI if the SQL connection string is missing
        if (!sqlConnectionStringFound)
            options.HeadContent = "<h1 align=\"center\">❌ SqlConnectionString not found ❌</h1>";
    });
}
else
{
    // Show the health message directly in non-development environments
    var buildTimeStamp = File.GetCreationTime(Assembly.GetExecutingAssembly().Location);
    string currentHealthMessage = $"The API is up 🚀 | Connection string found: {(sqlConnectionStringFound ? "✅" : "❌")} | Build timestamp: {buildTimeStamp}";

    app.MapGet("/", () => currentHealthMessage);
}

// Enforce HTTPS for all requests.
app.UseHttpsRedirection();

// Enable CORS for Unity/external clients
app.UseCors("AllowUnity");

// Enable authorization middleware.
app.UseAuthorization();

// Register Identity endpoints for account management (register, login, etc.) under /account.
app.MapGroup("/account").MapIdentityApi<IdentityUser>().WithTags("Account");

// Register all controller endpoints for the application.
app.MapControllers();

app.Run();