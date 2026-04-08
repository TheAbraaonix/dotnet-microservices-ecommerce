namespace PedidoService.Models;

public enum OrderStatus
{
    Pending = 0,
    StockReserved = 1,
    PaymentApproved = 2,
    Confirmed = 3,
    PaymentRejected = 4,
    Cancelled = 5,
    Failed = 6
}
