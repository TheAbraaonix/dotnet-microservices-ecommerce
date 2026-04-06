using Microsoft.AspNetCore.Mvc;

namespace PedidoService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(ILogger<OrdersController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("GET /api/orders called");
        return Ok(new { Message = "PedidoService is running!", Status = "Active" });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        // TODO: Implement get order by id
        return Ok(new { Id = id, Status = "NotImplemented" });
    }

    [HttpPost]
    public IActionResult Create([FromBody] object request)
    {
        // TODO: Implement create order
        return Ok(new { Message = "Order creation endpoint (coming soon)" });
    }
}
