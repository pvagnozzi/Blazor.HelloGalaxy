using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Blazor.HelloGalaxy.Server.Models;
using Blazor.HelloGalaxy.Server.Services;
using Blazor.HelloGalaxy.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Blazor.HelloGalaxy.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AccountsController(SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager, IUserService userService, IConfiguration configuration)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _userService = userService;
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

        var identityResult = await _userManager.CreateAsync(identityUser, user.Password);
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
    private Task<string> GeneraJsonWebTokenAsync(User identityUser)
    {
        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecurityKey"] ?? string.Empty));
        var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var roleNames = new[] { "Admin " };
        var email = identityUser.UserName!;

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, identityUser.Id),
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }.Union(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10),
            credentials
        );
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
    }

    [HttpPost]
    [Route("signin")]
    [AllowAnonymous]
    public async Task<ActionResult<SignInRespose>> SignInAsync([FromBody] RegisterRequest user)
    {
        //var signInResult = await _signInManager.PasswordSignInAsync(user.Email, user.Password, false, false);
        //if (!signInResult.Succeeded)
        //{
        //    return Unauthorized(user);
        //}

        
        //var newUser = new User("piergiorgio_vagnozzi@hotmail.com", "TestTest123!");
        //await _userService.RegisterAsync(newUser);

        var identityUser = await _userService.LoginAsync(user.Email, user.Password);

        //var identityUser = await _userManager.FindByEmailAsync(user.Email);
        var token = await GeneraJsonWebTokenAsync(identityUser!);
        var result = new SignInRespose
        {
            AccessToken = token
        };

        return Ok(result);
    }
}