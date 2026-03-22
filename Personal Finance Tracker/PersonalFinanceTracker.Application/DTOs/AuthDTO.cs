using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Application.DTOs
{
    public record RegisterRequest(string Username, string Email, string Password);

    public record LoginRequest(string Email, string Password);

    public record AuthResponse(string AccessToken, string RefreshToken, string Username);
}
