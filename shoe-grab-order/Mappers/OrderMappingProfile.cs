using AutoMapper;
using ShoeGrabCommonModels;
using ShoeGrabOrderManagement.Dto;

namespace ShoeGrabOrderManagement.Database.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderDto, Order>();


        CreateMap<CreateOrderDto, Order>();

        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<OrderItemCreateDto, OrderItem>();

        CreateMap<OrderItemDto, OrderItem>();
    }
}
