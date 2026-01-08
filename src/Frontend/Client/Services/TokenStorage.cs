using Microsoft.JSInterop;

namespace Frontend.Client.Services;

public class TokenStorage
{
    private const string TokenKey = "sav_token";
    private readonly IJSRuntime _jsRuntime;

    public TokenStorage(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("savStorage.get", TokenKey);
    }

    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("savStorage.set", TokenKey, token);
    }

    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeVoidAsync("savStorage.remove", TokenKey);
    }
}
