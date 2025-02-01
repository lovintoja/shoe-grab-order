using AutoMapper;
using ShoeGrabCommonModels;
using ShoeGrabOrderManagement.Dto;

namespace ShoeGrabOrderManagement.Database.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<Order, OrderDetailsDto>();
        CreateMap<Order, OrderSummaryDto>();
        CreateMap<Order, AdminOrderDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username));

        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<OrderItem, OrderItemExtendedDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        CreateMap<OrderItemCreateDto, OrderItem>();

        CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        CreateMap<OrderItemDto, OrderItem>()
            .ForMember(dest => dest.Product.Name, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Product.Id, opt => opt.MapFrom(src => src.ProductId));
        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

    }
}
