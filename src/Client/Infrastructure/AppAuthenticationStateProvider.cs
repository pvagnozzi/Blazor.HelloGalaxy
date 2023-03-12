using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor.HelloGalaxy.Client.Infrastructure;

public class AppAuthenticationStateProvider: AuthenticationStateProvider
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    private readonly TokenManager _tokenManager;
    
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
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
 
            var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            var expires = jwtSecurityToken.ValidTo;
            if (expires < DateTime.UtcNow)
            {
                await _tokenManager.ClearTokenAsync();
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
 
            var claims = jwtSecurityToken.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, jwtSecurityToken.Subject));
 
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
 
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task SignInAsync()
    {
        var savedToken = await _tokenManager.GetTokenAsync();

        var jwtSecurityToken = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
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

}
