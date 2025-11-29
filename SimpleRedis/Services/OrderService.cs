using Microsoft.EntityFrameworkCore;
using SimpleRedis.Data;
using SimpleRedis.Models;
using System.Text.Json;

namespace SimpleRedis.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IRedisCacheService _cacheService;
        private readonly ILogger<OrderService> _logger;
        public OrderService(AppDbContext context, IRedisCacheService cacheService, ILogger<OrderService> logger)
        {
            _context = context;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task<List<Order>> GetAllAsync()
        {
            const string cacheKey = "orders:all";
            var cached = await _cacheService.GetAsync<List<Order>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Retrieved all orders from cache.");
                return cached;
            }
            _logger.LogInformation("Retrieving all orders from database.");
            var orders = await _context.Orders.ToListAsync();
            await _cacheService.SetAsync(cacheKey, orders, TimeSpan.FromMinutes(10));
            return orders;
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"orders:{id}";
            var cached = await _cacheService.GetAsync<Order>(cacheKey);

            if (cached != null)
            {
               return cached;
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                await _cacheService.SetAsync(cacheKey, order, TimeSpan.FromMinutes(10));
            }
            return order;

        }

        public async Task<Order> CreateAsync(Order order)
        {
            order.Id = Guid.NewGuid();
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created new order with ID: {OrderId}", order.Id);
            await _cacheService.RemoveAsync("orders:all");
            return order;
        }

        public async Task<Order?> UpdateAsync(Guid id, Order order)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return null;
            }
            existingOrder.CustomerName = order.CustomerName;
            existingOrder.Amount = order.Amount;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated order with ID: {OrderId}", id);
            var cacheKey = $"orders:{id}";
            await _cacheService.RemoveAsync(cacheKey);
            await _cacheService.RemoveAsync("orders:all");
            return existingOrder;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return false;
            }
            _context.Orders.Remove(existingOrder);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted order with ID: {OrderId}", id);
            var cacheKey = $"orders:{id}";
            await _cacheService.RemoveAsync(cacheKey);
            await _cacheService.RemoveAsync("orders:all");
            return true;
        }
    }
}
