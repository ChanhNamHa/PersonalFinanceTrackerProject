using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService, ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        // 1. Kiểm tra email đã tồn tại chưa
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email đã được sử dụng.");

        // 2. Tạo User mới và Hash mật khẩu
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Đăng ký thành công!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // 1. Tìm User theo Email
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Email hoặc mật khẩu không chính xác.");

        // 2. Tạo Access Token và Refresh Token
        var accessToken = _authService.GenerateAccessToken(user);
        var refreshToken = _authService.GenerateRefreshToken();

        // 3. Lưu Refresh Token vào Database (Để kiểm tra sau này)
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync();

        return Ok(new AuthResponse(accessToken, refreshToken, user.Username));
    }
}