using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetOrders(int userId);
    Task<bool> CreateOrder(int userId, Order order);
}
