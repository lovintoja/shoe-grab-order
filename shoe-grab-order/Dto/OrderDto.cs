using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Dto;
public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public double TotalPrice { get; set; }
    public AddressDto ShippingAddress { get; set; } = new();
    public PaymentInfoDto PaymentInfo { get; set; } = new();
    public string ContactPhone { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}
