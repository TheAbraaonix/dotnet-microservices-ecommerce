namespace PedidoService.DTOs;

public record OrderItemResponse(Guid ProductId, int Quantity, decimal UnitPrice);
