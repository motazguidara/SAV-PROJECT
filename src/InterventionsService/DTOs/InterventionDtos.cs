using InterventionsService.Models;

namespace InterventionsService.DTOs;

public record CreateInterventionRequest(Guid ClaimId, string Notes, decimal LaborCost);
public record ScheduleInterventionRequest(DateTime ScheduledAt);
public record LaborCostRequest(decimal LaborCost);
public record AddPartRequest(string PartName, decimal UnitPrice, int Qty);

public record InterventionResponse(Guid Id, Guid ClaimId, DateTime? ScheduledAt, DateTime? StartedAt, DateTime? EndedAt,
    InterventionStatus Status, bool IsUnderWarranty, decimal LaborCost, decimal InvoiceTotal, string Notes);

public record PartLineResponse(Guid Id, Guid InterventionId, string PartName, decimal UnitPrice, int Qty, decimal LineTotal);

public record InvoiceResponse(Guid InterventionId, bool IsUnderWarranty, decimal LaborCost, decimal PartsTotal, decimal InvoiceTotal, IReadOnlyCollection<PartLineResponse> Lines);
