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
        CreateMap<Order, AdminOrderDto>();

        CreateMap<CreateOrderDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.Ignore());

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<OrderItem, OrderItemExtendedDto>()
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        CreateMap<OrderItemCreateDto, OrderItem>();

        CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));
        CreateMap<OrderItemDto, OrderItem>();
        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

    }
}
