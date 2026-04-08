using PedidoService.Models;

namespace PedidoService.DTOs;

public record OrderResponse(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal Total,
    DateTime CreatedAt,
    List<OrderItemResponse> Items)
{
    public static OrderResponse From(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status.ToString(),
            order.Total,
            order.CreatedAt,
            order.Items.Select(i => new OrderItemResponse(i.ProductId, i.Quantity, i.UnitPrice)).ToList()
        );
}
