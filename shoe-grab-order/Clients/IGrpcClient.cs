using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Clients;

public interface IGrpcClient
{
    Task<User> GetUser(int id);
    Task<Product> GetProduct(int id);
    Task<bool> SendOrderSentNotificationEmail(string recepientEmail);
}
