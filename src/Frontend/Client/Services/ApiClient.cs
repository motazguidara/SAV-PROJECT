using System.Net.Http.Headers;
using System.Net.Http.Json;
using Frontend.Shared;

namespace Frontend.Client.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly TokenStorage _storage;

    public ApiClient(HttpClient httpClient, TokenStorage storage)
    {
        _httpClient = httpClient;
        _storage = storage;
    }

    private async Task AddAuthHeaderAsync()
    {
        var token = await _storage.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/identity/api/v1/auth/register", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>() ?? throw new InvalidOperationException();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/identity/api/v1/auth/login", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>() ?? throw new InvalidOperationException();
    }

    public async Task<UserProfile> GetProfileAsync()
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.GetFromJsonAsync<UserProfile>("/identity/api/v1/me");
        return response ?? throw new InvalidOperationException();
    }

    public async Task<IReadOnlyCollection<ArticleResponse>> GetArticlesAsync()
    {
        await AddAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<ArticleResponse>>("/catalog/api/v1/articles") ?? Array.Empty<ArticleResponse>();
    }

    public async Task CreateArticleAsync(CreateArticleRequest request)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/catalog/api/v1/articles", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeactivateArticleAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsync($"/catalog/api/v1/articles/{id}/deactivate", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateClaimAsync(CreateClaimRequest request)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/claims/api/v1/claims", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<ClaimResponse>> GetClaimsAsync(string? status = null)
    {
        await AddAuthHeaderAsync();
        var url = string.IsNullOrWhiteSpace(status) ? "/claims/api/v1/claims" : $"/claims/api/v1/claims?status={status}";
        return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<ClaimResponse>>(url) ?? Array.Empty<ClaimResponse>();
    }

    public async Task<ClaimResponse?> GetClaimAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<ClaimResponse>($"/claims/api/v1/claims/{id}");
    }

    public async Task UpdateClaimStatusAsync(Guid id, ClaimStatus status)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsJsonAsync($"/claims/api/v1/claims/{id}/status", new { Status = status });
        response.EnsureSuccessStatusCode();
    }

    public async Task<InterventionResponse> CreateInterventionAsync(CreateInterventionRequest request)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/interventions/api/v1/interventions", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InterventionResponse>() ?? throw new InvalidOperationException();
    }

    public async Task<IReadOnlyCollection<InterventionResponse>> GetInterventionsByClaimAsync(Guid claimId)
    {
        await AddAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<IReadOnlyCollection<InterventionResponse>>($"/interventions/api/v1/interventions/by-claim/{claimId}") ?? Array.Empty<InterventionResponse>();
    }

    public async Task<InterventionResponse?> GetInterventionAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<InterventionResponse>($"/interventions/api/v1/interventions/{id}");
    }

    public async Task ScheduleInterventionAsync(Guid id, DateTime scheduledAt)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsJsonAsync($"/interventions/api/v1/interventions/{id}/schedule", new ScheduleInterventionRequest(scheduledAt));
        response.EnsureSuccessStatusCode();
    }

    public async Task StartInterventionAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsync($"/interventions/api/v1/interventions/{id}/start", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task EndInterventionAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsync($"/interventions/api/v1/interventions/{id}/end", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task AddPartAsync(Guid id, AddPartRequest request)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync($"/interventions/api/v1/interventions/{id}/parts", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateLaborAsync(Guid id, decimal labor)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PatchAsJsonAsync($"/interventions/api/v1/interventions/{id}/labor", new LaborCostRequest(labor));
        response.EnsureSuccessStatusCode();
    }

    public async Task<InvoiceResponse?> GetInvoiceAsync(Guid id)
    {
        await AddAuthHeaderAsync();
        return await _httpClient.GetFromJsonAsync<InvoiceResponse>($"/interventions/api/v1/interventions/{id}/invoice");
    }
}
