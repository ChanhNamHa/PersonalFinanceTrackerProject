using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Application.DTOs
{
    // Request: Dữ liệu nhận vào khi tạo/cập nhật
    public record CreateBudgetRequest(
        [Range(0.01, double.MaxValue, ErrorMessage = "Hạn mức phải lớn hơn 0")]
        decimal LimitAmount,

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        DateTime StartDate,

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        DateTime EndDate,

        [Required(ErrorMessage = "Danh mục không được để trống")]
        Guid CategoryId
    );

    // Response: Dữ liệu trả về cho Frontend
    public record BudgetResponse(
        Guid Id,
        decimal LimitAmount,
        DateTime StartDate,
        DateTime EndDate,
        string CategoryName,
        Guid CategoryId,
        decimal CurrentSpent // Số tiền thực tế đã tiêu trong kỳ
    );
}