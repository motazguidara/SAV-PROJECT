namespace InterventionsService.Models;

public class PartLine
{
    public Guid Id { get; set; }
    public Guid InterventionId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Qty { get; set; }
    public decimal LineTotal { get; set; }

    public Intervention? Intervention { get; set; }
}
