using SimpleRedis.Models;

namespace SimpleRedis.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order> CreateAsync(Order order);
        Task<Order?> UpdateAsync(Guid id, Order order);
        Task<bool> DeleteAsync(Guid id);
    }
}
