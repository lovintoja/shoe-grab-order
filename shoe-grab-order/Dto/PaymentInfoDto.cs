namespace ShoeGrabOrderManagement.Dto;

public class PaymentInfoDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
