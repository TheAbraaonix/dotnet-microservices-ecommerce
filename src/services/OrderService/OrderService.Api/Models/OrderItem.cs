namespace PedidoService.Models;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid OrderId { get; private set; }
    public Order? Order { get; private set; }

    private OrderItem() { }

    public OrderItem(Guid productId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(unitPrice));

        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Id = Guid.NewGuid();
    }

    public decimal CalculateTotal() => Quantity * UnitPrice;
}
