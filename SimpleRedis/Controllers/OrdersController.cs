using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleRedis.Models;
using SimpleRedis.Services;

namespace SimpleRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {   
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found.", id);
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Create(Order order)
        {
            var createdOrder = await _orderService.CreateAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> Update(Guid id, Order order)
        {
            var updatedOrder = await _orderService.UpdateAsync(id, order);
            if (updatedOrder == null)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for update.", id);
                return NotFound();
            }
            return Ok(updatedOrder);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _orderService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Order with ID: {OrderId} not found for deletion.", id);
                return NotFound();
            }
            return NoContent();
        }

    }
}
