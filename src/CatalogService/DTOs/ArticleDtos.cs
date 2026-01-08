namespace CatalogService.DTOs;

public record ArticleResponse(Guid Id, string Name, string Brand, int WarrantyMonths, bool Active);
public record CreateArticleRequest(string Name, string Brand, int WarrantyMonths);
