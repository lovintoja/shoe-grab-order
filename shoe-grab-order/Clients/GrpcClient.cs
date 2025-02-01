using AutoMapper;
using ShoeGrabCommonModels;
using static CrmService;
using static ProductManagement;
using static UserManagement;

namespace ShoeGrabOrderManagement.Clients;

public class GrpcClient : IGrpcClient
{
    private readonly UserManagementClient _userManagementClient;
    private readonly ProductManagementClient _productManagementClient;
    private readonly CrmServiceClient _crmServiceClient;
    private readonly IMapper _mapper;

    public GrpcClient(UserManagementClient userManagementClient, 
        ProductManagementClient productManagementClient, 
        CrmServiceClient crmServiceClient,
        IMapper mapper)
    {
        _userManagementClient = userManagementClient;
        _productManagementClient = productManagementClient;
        _crmServiceClient = crmServiceClient;
        _mapper = mapper;
    }

    public async Task<Product> GetProduct(int id)
    {
        var request = new GetProductRequest { Id = id };
        var response = await _productManagementClient.GetProductAsync(request).ResponseAsync;
        return _mapper.Map<Product>(response.Product);
    }

    public async Task<User> GetUser(int id)
    {
        var request = new GetUserRequest { Id = id };
        var response = await _userManagementClient.GetUserAsync(request).ResponseAsync;
        return _mapper.Map<User>(response.User);
    }

    public async Task<bool> SendOrderSentNotificationEmail(string recepientEmail)
    {
        var request = new SendNotificationEmailRequest { RecepientEmail = recepientEmail };
        var response = await _crmServiceClient.SendNotificationEmailAsync(request).ResponseAsync;
        
        if (response?.Result == "Success")
        {
            return true;
        }
        return false;
    }
}
