using PedidoService.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Register all services via extension method
builder.Services.AddDependencies(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handler (must be before controllers)
app.UseMiddleware<GlobalExceptionHandler>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Health check endpoint
app.MapHealthChecks("/health");

// Root endpoint
app.MapGet("/", () => new { Service = "PedidoService", Status = "Running", Version = "1.0.0" });

app.Run();
