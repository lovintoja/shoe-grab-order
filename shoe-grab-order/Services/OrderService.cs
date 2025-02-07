using Microsoft.EntityFrameworkCore;
using ShoeGrabCommonModels;
using ShoeGrabCommonModels.Contexts;
using ShoeGrabOrderManagement.Clients;


namespace ShoeGrabOrderManagement.Services;

public class OrderService : IOrderService
{
    private readonly OrderContext _context;
    private readonly IGrpcClient _grpcClient;

    public OrderService(OrderContext context, IGrpcClient grpcClient)
    {
        _context = context;
        _grpcClient = grpcClient;
    }

    public async Task<bool> CreateOrder(int userId, Order order)
    {
        foreach (var item in order.Items)
        {
            var product = await _grpcClient.GetProduct(item.ProductId);
            if (product == null)
                throw new ArgumentException("Cannot find specified product.");

            item.UnitPrice = product.Price;
            item.ProductName = product.Name;
        }

        order.TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice);
        order.UserId = userId;
        try
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return false;
        }

        try
        {
            var user = await _grpcClient.GetUser(userId);
            await _grpcClient.SendOrderSentNotificationEmail(user.Email);
        }
        catch (Exception)
        {
            //If email does not get sent, order still went through.
            return true;
        }
        return true;
    }

    public async Task<IEnumerable<Order>> GetOrders(int userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ToListAsync();
    }
}
