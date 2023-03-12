using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazor.HelloGalaxy.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Blazor.HelloGalaxy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly SignInManager<IdentityUser> signInManager;
    private readonly UserManager<IdentityUser> userManager;
    private readonly IConfiguration configuration;

    public AccountsController(SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.configuration = configuration;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest user)
    {
        var identityUser = new IdentityUser
        {
            Email = user.Email,
            UserName = user.Email
        };

        var identityResult = await userManager.CreateAsync(identityUser, user.Password);
        if (identityResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status201Created, new { identityResult.Succeeded });
        }

        var errorsToReturn = new StringBuilder("Registration failed");
        foreach (var error in identityResult.Errors)
        {
            errorsToReturn.Append(Environment.NewLine);
            errorsToReturn.Append($"Codice di errore: {error.Code}, {error.Description}");
        }

        return StatusCode(StatusCodes.Status500InternalServerError, errorsToReturn);
    }

    [NonAction]
    [ApiExplorerSettings(IgnoreApi = true)]
    private async Task<string> GeneraJSONWebToken(IdentityUser identityUser)
    {
        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"] ?? string.Empty));
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var roleNames = await userManager.GetRolesAsync(identityUser);

        var email = identityUser.Email!;

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, identityUser.Id),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Union(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSecurityToken = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10),
            credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }

    [HttpPost]
    [Route("signin")]
    [AllowAnonymous]
    public async Task<ActionResult<SignInRespose>> SignInAsync([FromBody] RegisterRequest user)
    {
        var signInResult = await signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);
        if (!signInResult.Succeeded)
        {
            return Unauthorized(user);
        }

        var identityUser = await userManager.FindByEmailAsync(user.Email);
        var token = await GeneraJSONWebToken(identityUser!);

        var result = new SignInRespose
        {
            AccessToken = token
        };

        return Ok(result);
    }
}