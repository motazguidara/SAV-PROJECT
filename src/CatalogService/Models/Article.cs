namespace CatalogService.Models;

public class Article
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public int WarrantyMonths { get; set; }
    public bool Active { get; set; } = true;
}
