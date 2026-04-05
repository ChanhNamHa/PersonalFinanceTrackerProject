using System;

namespace PersonalFinanceTracker.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? GetUserId();
    }
}
