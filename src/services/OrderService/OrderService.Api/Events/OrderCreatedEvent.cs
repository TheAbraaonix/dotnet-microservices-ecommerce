namespace PedidoService.Events;

public class OrderCreatedEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string EventType => "OrderCreated";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public OrderCreatedData Data { get; set; } = new();
}

public class OrderCreatedData
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemData> Items { get; set; } = new();
}

public class OrderItemData
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
