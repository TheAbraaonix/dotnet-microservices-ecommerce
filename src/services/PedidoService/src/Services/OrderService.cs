using Microsoft.EntityFrameworkCore;
using PedidoService.DTOs;
using PedidoService.Events;
using PedidoService.Exceptions;
using PedidoService.Infrastructure.Data;
using PedidoService.Models;

namespace PedidoService.Services;

public class OrderService : IOrderService
{
    private readonly OrderDbContext _context;
    private readonly IKafkaProducer _kafkaProducer;
    private readonly string _ordersTopic;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrderDbContext context,
        IKafkaProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<OrderService> logger)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
        _ordersTopic = configuration["Kafka:Topics:Orders"]!;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default)
    {
        var items = request.Items
            .Select(i => new OrderItem(i.ProductId, i.Quantity, i.UnitPrice))
            .ToList();

        var order = new Order(request.CustomerId, items);

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} created for customer {CustomerId} with total {Total}",
            order.Id, order.CustomerId, order.Total);

        var orderCreatedEvent = new OrderCreatedEvent
        {
            Data = new OrderCreatedData
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                Total = order.Total,
                Items = order.Items.Select(i => new OrderItemData
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            }
        };

        await _kafkaProducer.PublishAsync(_ordersTopic, order.Id.ToString(), orderCreatedEvent, ct);

        return OrderResponse.From(order);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        if (order == null)
            throw new NotFoundException("order", id);

        return OrderResponse.From(order);
    }

    public async Task<IEnumerable<OrderResponse>> ListAsync(CancellationToken ct = default)
    {
        var orders = await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

        return orders.Select(OrderResponse.From);
    }
}
