using PedidoService.DTOs;
using PedidoService.Models;

namespace PedidoService.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken ct = default);
    Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<OrderResponse>> ListAsync(CancellationToken ct = default);
}
