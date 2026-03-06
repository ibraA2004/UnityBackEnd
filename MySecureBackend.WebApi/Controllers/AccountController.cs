using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
[Consumes("application/json")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new IdentityUser
        {
            UserName = request.Username ?? request.Email, // Als Username leeg is, gebruik Email
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        return Ok(new { message = "User registered successfully", userId = user.Id, username = user.UserName });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Unity stuurt usernameOrEmail via User object
        var usernameOrEmail = user.UsernameOrEmail;
        IdentityUser? identityUser = null;

        // Try to find by email first
        if (usernameOrEmail.Contains("@"))
        {
            identityUser = await _userManager.FindByEmailAsync(usernameOrEmail);
        }

        // If not found by email, try username
        if (identityUser == null)
        {
            identityUser = await _userManager.FindByNameAsync(usernameOrEmail);
        }

        if (identityUser == null)
        {
            return Unauthorized(new { message = "Invalid username/email or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(identityUser, user.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid username/email or password" });
        }

        var token = await _jwtService.GenerateJwtTokenAsync(identityUser);

        // Return token in format compatible with JsonHelper.ExtractToken
        return Ok(new
        {
            token = token,
            userId = identityUser.Id,
            username = identityUser.UserName,
            email = identityUser.Email
        });
    }
}

// Models compatible with Unity
public class RegisterRequest
{
    public string? Username { get; set; } // Optioneel: als leeg, wordt Email gebruikt
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class User
{
    public required string UsernameOrEmail { get; set; }
    public required string Password { get; set; }
}
