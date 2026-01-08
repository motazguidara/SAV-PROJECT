using System.Net.Http.Json;

namespace InterventionsService.Services;

public record ClaimDetail(Guid Id, Guid CustomerId, Guid ArticleId, DateOnly PurchaseDate, string Status);

public class ClaimsClient
{
    private readonly HttpClient _httpClient;

    public ClaimsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClaimDetail?> GetClaimAsync(Guid claimId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/v1/claims/{claimId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ClaimDetail>(cancellationToken: cancellationToken);
    }

    public async Task UpdateClaimStatusAsync(Guid claimId, string status, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/v1/claims/{claimId}/status", new { Status = status }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
