using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceTracker.Application.DTOs
{
    // RegisterRequest.cs
    public record RegisterRequest(string Username, string Email, string Password);

    // LoginRequest.cs
    public record LoginRequest(string Email, string Password);

    // AuthResponse.cs (Dữ liệu trả về sau khi Login thành công)
    public record AuthResponse(string AccessToken, string RefreshToken, string Username);
}
