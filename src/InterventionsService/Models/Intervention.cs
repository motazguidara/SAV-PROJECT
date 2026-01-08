namespace InterventionsService.Models;

public class Intervention
{
    public Guid Id { get; set; }
    public Guid ClaimId { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public InterventionStatus Status { get; set; } = InterventionStatus.Planned;
    public bool IsUnderWarranty { get; set; }
    public decimal LaborCost { get; set; }
    public decimal InvoiceTotal { get; set; }
    public string Notes { get; set; } = string.Empty;
    public ICollection<PartLine> PartLines { get; set; } = new List<PartLine>();
}
