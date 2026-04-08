using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Services;
using System.Text.RegularExpressions;

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

        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new { message = "Username is required" });

        if (!IsUsernameValid(request.Username))
            return BadRequest(new { message = "Username must be alphanumeric (letters and digits) and contain no spaces." });

        if (!IsPasswordValid(request.Password, out var pwdError))
            return BadRequest(new { message = pwdError });

        var user = new IdentityUser
        {
            UserName = request.Username
            // Email intentionally not set - registrations use username only
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // Return precise backend errors to the client (e.g. username already exists)
            var errors = result.Errors.Select(e => e.Description).ToArray();
            return BadRequest(new { message = "Registration failed", errors });
        }

        return Ok(new { message = "User registered successfully", userId = user.Id, username = user.UserName });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] User user)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Client sends only 'username' and 'password'
        if (string.IsNullOrWhiteSpace(user.Username))
            return BadRequest(new { message = "Username is required" });

        var identityUser = await _userManager.FindByNameAsync(user.Username);
        if (identityUser == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(identityUser, user.Password, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid username or password" });
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

    // Validation helpers
    private static bool IsUsernameValid(string username)
    {
        // Only letters and digits allowed, at least 1 character
        return Regex.IsMatch(username, @"^[A-Za-z0-9]+$");
    }

    private static bool IsPasswordValid(string password, out string error)
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(password) || password.Length < 10)
        {
            error = "Password must be at least 10 characters long.";
            return false;
        }
        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            error = "Password must contain at least one lowercase letter.";
            return false;
        }
        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            error = "Password must contain at least one uppercase letter.";
            return false;
        }
        if (!Regex.IsMatch(password, @"\d"))
        {
            error = "Password must contain at least one digit.";
            return false;
        }
        if (!Regex.IsMatch(password, @"\W"))
        {
            error = "Password must contain at least one non-alphanumeric character.";
            return false;
        }
        return true;
    }
}

// Models compatible with Unity
public class RegisterRequest
{
    // Username is required. Email is not used by this backend.
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class User
{
    // Clients send only 'username' now
    public string? Username { get; set; }
    public required string Password { get; set; }
}