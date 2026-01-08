namespace Frontend.Shared;

public record RegisterRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string AccessToken, DateTime ExpiresAtUtc);
public record UserProfile(string Id, string Email, string FullName, IReadOnlyCollection<string> Roles);

public record ArticleResponse(Guid Id, string Name, string Brand, int WarrantyMonths, bool Active);
public record CreateArticleRequest(string Name, string Brand, int WarrantyMonths);

public enum ClaimStatus
{
    New,
    InProgress,
    Resolved,
    Rejected
}

public record CreateClaimRequest(Guid ArticleId, string SerialNumber, DateOnly PurchaseDate, string Description);
public record ClaimResponse(Guid Id, Guid CustomerId, Guid ArticleId, string SerialNumber, DateOnly PurchaseDate, string Description, ClaimStatus Status, DateTime CreatedAt, DateTime UpdatedAt);

public enum InterventionStatus
{
    Planned,
    InProgress,
    Done,
    Canceled
}

public record CreateInterventionRequest(Guid ClaimId, string Notes, decimal LaborCost);
public record ScheduleInterventionRequest(DateTime ScheduledAt);
public record LaborCostRequest(decimal LaborCost);
public record AddPartRequest(string PartName, decimal UnitPrice, int Qty);
public record InterventionResponse(Guid Id, Guid ClaimId, DateTime? ScheduledAt, DateTime? StartedAt, DateTime? EndedAt, InterventionStatus Status, bool IsUnderWarranty, decimal LaborCost, decimal InvoiceTotal, string Notes);
public record PartLineResponse(Guid Id, Guid InterventionId, string PartName, decimal UnitPrice, int Qty, decimal LineTotal);
public record InvoiceResponse(Guid InterventionId, bool IsUnderWarranty, decimal LaborCost, decimal PartsTotal, decimal InvoiceTotal, IReadOnlyCollection<PartLineResponse> Lines);
