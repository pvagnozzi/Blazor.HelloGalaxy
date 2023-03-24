using System.Diagnostics;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor.HelloGalaxy.Client.Infrastructure;

public class AppAuthenticationStateProvider: AuthenticationStateProvider
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly TokenManager _tokenManager;
    private readonly ClaimsPrincipal _nobody = new(new ClaimsIdentity());
    
    public AppAuthenticationStateProvider(TokenManager tokenManager)
    {
        _tokenManager = tokenManager;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var savedToken = await _tokenManager.GetTokenAsync();
            if (string.IsNullOrEmpty(savedToken))
            {
                return Nobody();
            }
 
            var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var expires = jwtSecurityToken.ValidTo;
            if (expires < DateTime.UtcNow)
            {
                await _tokenManager.ClearTokenAsync();
                return Nobody();
            }
 
            var claims = jwtSecurityToken.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, jwtSecurityToken.Subject));
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return Nobody();
        }
        catch
        {
            return new AuthenticationState(_nobody);
        }
    }

    public async Task SignInAsync(string? token = null, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(token))
        {
            await _tokenManager.SetTokenAsync(token, cancellationToken);
        }
        else
        {
            token = await _tokenManager.GetTokenAsync(cancellationToken);
        }
        
        
        var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
        var claims = jwtSecurityToken.Claims.ToList();
        claims.Add(new Claim(ClaimTypes.Name, jwtSecurityToken.Subject));
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
        var authentication = Task.FromResult(new AuthenticationState(user));
        NotifyAuthenticationStateChanged(authentication);
    }

    public void SignOutAsync()
    {
        var nobody = new ClaimsPrincipal(new ClaimsIdentity());
        _tokenManager.ClearTokenAsync();
        var authentication = Task.FromResult(new AuthenticationState(nobody));
        NotifyAuthenticationStateChanged(authentication);
    }

    [DebuggerStepThrough]
    private AuthenticationState Nobody() => new(_nobody);
}
