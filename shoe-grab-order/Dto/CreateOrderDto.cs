using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Dto;
public class CreateOrderDto
{
    public AddressDto ShippingAddress { get; set; }
    public PaymentInfoDto PaymentInfo { get; set; }
    public string ContactPhone { get; set; }
    public string? Note { get; set; }
    public List<OrderItemCreateDto> Items { get; set; } = new();
}