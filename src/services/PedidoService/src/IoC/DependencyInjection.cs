using Microsoft.EntityFrameworkCore;
using PedidoService.Middleware;
using PedidoService.Services;
using PedidoService.Infrastructure.Data;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering PedidoService dependencies.
/// Usage: builder.Services.AddDependencies(builder.Configuration);
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core + PostgreSQL
        services.AddDbContext<OrderDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Application services
        services.AddScoped<IOrderService, OrderService>();

        // Kafka producer
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        // Middleware
        services.AddTransient<GlobalExceptionHandler>();

        // Health checks
        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Default")!);

        return services;
    }
}
