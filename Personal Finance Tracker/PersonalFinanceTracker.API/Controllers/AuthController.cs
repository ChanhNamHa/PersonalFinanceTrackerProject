using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
    {
        _authService = authService;

        // refresh service is resolved from DI if available
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        try
        {
            var response = await _refreshTokenService.RefreshTokenAsync(request.RefreshToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}