using AutoMapper;
using static CrmService;
using static ProductManagement;
using static UserManagement;
using CommonUser = ShoeGrabCommonModels.User;
using CommonProduct = ShoeGrabCommonModels.Product;

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

    public async Task<CommonProduct> GetProduct(int id)
    {
        var request = new GetProductRequest { Id = id };
        var response = await _productManagementClient.GetProductAsync(request).ResponseAsync;
        return _mapper.Map<CommonProduct>(response.Product);
    }

    public async Task<CommonUser> GetUser(int id)
    {
        var request = new GetUserRequest { Id = id };
        var response = await _userManagementClient.GetUserAsync(request).ResponseAsync;
        return _mapper.Map<CommonUser>(response.User);
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
