using CommonUser = ShoeGrabCommonModels.User;
using CommonProduct = ShoeGrabCommonModels.Product;

namespace ShoeGrabOrderManagement.Clients;

public interface IGrpcClient
{
    Task<CommonUser> GetUser(int id);
    Task<CommonProduct> GetProduct(int id);
    Task<bool> SendOrderSentNotificationEmail(string recepientEmail);
}
