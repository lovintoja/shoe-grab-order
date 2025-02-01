using ShoeGrabCommonModels;

namespace ShoeGrabOrderManagement.Dto;
public class OrderDetailsDto
{
    public int Id { get; set; }
    public string Status { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public Address ShippingAddress { get; set; }
    public PaymentInfo PaymentInfo { get; set; }
    public string ContactPhone { get; set; }
    public string? Note { get; set; }
    public int UserId { get; set; }
    public List<OrderItemExtendedDto> Items { get; set; }
}
