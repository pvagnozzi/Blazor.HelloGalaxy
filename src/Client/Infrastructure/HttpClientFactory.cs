using System.Net.Http.Headers;

namespace Blazor.HelloGalaxy.Client.Infrastructure;

public class HttpClientFactory
{
    private readonly TokenManager _tokenManager;

    private readonly string _baseAddress;

    public HttpClientFactory(string baseAddress, TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
        _baseAddress = baseAddress;
    }

    public async Task<HttpClient> GetClientAsync(CancellationToken cancellationToken = default)
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri(_baseAddress);
        var token = await _tokenManager.GetTokenAsync(cancellationToken);
        if (token is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }
        return client;
    }

}

