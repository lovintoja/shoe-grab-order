using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Dto;
public class CreateOrderDto
{
    public Address ShippingAddress { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
    public string ContactPhone { get; set; }
    public string? Note { get; set; }
    public List<OrderItemCreateDto> Items { get; set; } = new();
}