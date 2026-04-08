var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Health endpoint
app.MapGet("/", () => "API is alive 🚀");

app.UseHttpsRedirection();
app.Run();