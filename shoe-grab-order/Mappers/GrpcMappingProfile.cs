using AutoMapper;
using CommonProduct = ShoeGrabCommonModels.Product;
using CommonUser = ShoeGrabCommonModels.User;

namespace ShoeGrabOrderManagement.Database.Mappers;

public class GrpcMappingProfile : Profile
{
    public GrpcMappingProfile()
    {
        CreateMap<Product, CommonProduct>();
        CreateMap<User, CommonUser>();
    }
}
