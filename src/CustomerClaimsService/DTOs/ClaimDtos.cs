using CustomerClaimsService.Models;

namespace CustomerClaimsService.DTOs;

public record CreateClaimRequest(Guid ArticleId, string SerialNumber, DateOnly PurchaseDate, string Description);
public record ClaimResponse(Guid Id, Guid CustomerId, Guid ArticleId, string SerialNumber, DateOnly PurchaseDate, string Description, ClaimStatus Status, DateTime CreatedAt, DateTime UpdatedAt);
public record UpdateClaimStatusRequest(ClaimStatus Status);
