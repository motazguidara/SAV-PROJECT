namespace CustomerClaimsService.Models;

public class Claim
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ArticleId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; } = ClaimStatus.New;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
}
