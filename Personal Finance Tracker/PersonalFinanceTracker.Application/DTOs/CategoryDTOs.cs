namespace PersonalFinanceTracker.Application.DTOs
{
    public record CreateCategoryRequest(string Name, string Type);
    public record CategoryResponse(string Id, string Name, string Type);
}