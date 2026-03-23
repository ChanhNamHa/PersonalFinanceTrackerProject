using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Application.DTOs
{
    public record CreateCategoryRequest(
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục từ 2 đến 100 ký tự")]
        string Name,

        [Required(ErrorMessage = "Loại danh mục là bắt buộc")]
        string Type // True: Chi phí, False: Thu nhập
    );

    public record CategoryResponse(
        Guid Id,
        string Name,
        string Type
    );
}