using System.Net.Http.Json;

namespace InterventionsService.Services;

public record ArticleDetail(Guid Id, string Name, string Brand, int WarrantyMonths, bool Active);

public class CatalogClient
{
    private readonly HttpClient _httpClient;

    public CatalogClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ArticleDetail?> GetArticleAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<ArticleDetail>($"/api/v1/articles/{id}", cancellationToken);
    }
}
