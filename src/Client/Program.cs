using Blazor.HelloGalaxy.Client;
using Blazor.HelloGalaxy.Client.Infrastructure;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped<TokenManager>();
builder.Services.AddScoped(ctx => new HttpClientFactory(baseAddress, ctx.GetRequiredService<TokenManager>()));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
 
builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<AppAuthenticationStateProvider>());

await builder.Build().RunAsync();
