using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ShoeGrabCommonModels;


namespace ShoeGrabOrderManagement.Database.Mappers;

public class GrpcMappingProfile : Profile
{
    public GrpcMappingProfile()
    {
        CreateMap<ProductProto, Product>();
        CreateMap<UserProto, User>();
        CreateMap<UserProfileProto, UserProfile>();

        CreateMap<Order, OrderProto>()
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.OrderDate)));
        CreateMap<Address, AddressProto>();
        CreateMap<OrderItem, OrderItemProto>();
        CreateMap<PaymentInfo, PaymentInfoProto>()
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.PaymentDate)));
    }
}
