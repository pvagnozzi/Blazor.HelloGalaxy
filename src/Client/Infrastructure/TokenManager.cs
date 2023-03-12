using System.Diagnostics;
using Blazored.LocalStorage;

namespace Blazor.HelloGalaxy.Client.Infrastructure;

public class TokenManager
{
    private const string TokenKey = "bearerToken";

    private readonly ILocalStorageService _localStorageService;

    public TokenManager(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    [DebuggerStepThrough]
    public ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default) =>
        _localStorageService.GetItemAsStringAsync(TokenKey, cancellationToken);

    [DebuggerStepThrough]
    public ValueTask SetTokenAsync(string value, CancellationToken cancellationToken = default) =>
        _localStorageService.SetItemAsStringAsync(TokenKey, value, cancellationToken);

    [DebuggerStepThrough]
    public ValueTask ClearTokenAsync(CancellationToken cancellationToken = default) =>
        _localStorageService.RemoveItemAsync(TokenKey, cancellationToken);
}

