using PersonalFinanceTracker.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PersonalFinanceTracker.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        private readonly IUnitOfWork _uow;


    }
}
