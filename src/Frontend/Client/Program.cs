using Frontend.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

var apiBase = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBase) });
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, SavAuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
