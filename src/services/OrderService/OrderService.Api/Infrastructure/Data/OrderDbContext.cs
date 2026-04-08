using Microsoft.EntityFrameworkCore;
using PedidoService.Models;

namespace PedidoService.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(builder =>
        {
            builder.ToTable("orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(o => o.CustomerId).HasColumnName("customer_id").IsRequired();
            builder.Property(o => o.Status).HasColumnName("status").HasConversion<string>();
            builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.ToTable("order_items");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasColumnName("id").ValueGeneratedNever();
            builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
            builder.Property(i => i.Quantity).HasColumnName("quantity").IsRequired();
            builder.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).IsRequired();
            builder.Property(i => i.OrderId).HasColumnName("order_id").IsRequired();
        });
    }
}
