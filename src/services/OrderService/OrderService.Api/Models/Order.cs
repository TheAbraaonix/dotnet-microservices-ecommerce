namespace PedidoService.Models;

public class Order
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();

    public decimal Total => Items.Sum(i => i.CalculateTotal());

    private Order() { }

    public Order(Guid customerId, IEnumerable<OrderItem> items)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID is required", nameof(customerId));

        var orderItems = items.ToList();
        if (orderItems.Count == 0)
            throw new ArgumentException("At least one item is required", nameof(items));

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Items = orderItems;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.PaymentApproved)
            throw new InvalidOperationException($"Cannot confirm order in {Status} status");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed()
    {
        Status = OrderStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed)
            throw new InvalidOperationException("Cannot cancel a confirmed order");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReserveStock()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Cannot reserve stock in {Status} status");

        Status = OrderStatus.StockReserved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApprovePayment()
    {
        if (Status != OrderStatus.StockReserved)
            throw new InvalidOperationException($"Cannot approve payment in {Status} status");

        Status = OrderStatus.PaymentApproved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RejectPayment()
    {
        if (Status != OrderStatus.StockReserved)
            throw new InvalidOperationException($"Cannot reject payment in {Status} status");

        Status = OrderStatus.PaymentRejected;
        UpdatedAt = DateTime.UtcNow;
    }
}
