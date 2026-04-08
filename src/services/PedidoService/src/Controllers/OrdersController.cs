using Microsoft.AspNetCore.Mvc;
using PedidoService.DTOs;
using PedidoService.Services;

namespace PedidoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var response = await _orderService.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            ApiResponse<OrderResponse>.SuccessResponse(response, "Order created successfully", 201));
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<OrderResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var response = await _orderService.GetByIdAsync(id, ct);
        return Ok(ApiResponse<OrderResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// List all orders (newest first)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderResponse>>), 200)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var responses = await _orderService.ListAsync(ct);
        return Ok(ApiResponse<IEnumerable<OrderResponse>>.SuccessResponse(responses));
    }
}
