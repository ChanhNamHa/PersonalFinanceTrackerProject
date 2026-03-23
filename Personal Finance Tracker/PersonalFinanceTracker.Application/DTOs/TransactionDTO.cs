using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PersonalFinanceTracker.Application.DTOs
{
    public record CreateTransactionRequest
    {
        [Required(ErrorMessage = "Số tiền là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [MaxLength(500, ErrorMessage = "Ghi chú không được quá 500 ký tự")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "Ngày giao dịch là bắt buộc")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public Guid CategoryId { get; set; }
    }
    public record TransactionDTO
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        // Constructor để Map nhanh từ Entity sang DTO
        public TransactionDTO(Guid id, decimal amount, string? note, DateTime date, string categoryName, Guid categoryId)
        {
            Id = id;
            Amount = amount;
            Note = note;
            TransactionDate = date;
            CategoryName = categoryName;
            CategoryId = categoryId;
        }
    }
    public record UpdateTransactionRequest(
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền cập nhật phải lớn hơn 0")]
        decimal Amount,

        [MaxLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        string? Note,

        [Required(ErrorMessage = "Ngày giao dịch là bắt buộc")]
        DateTime TransactionDate,

        [Required(ErrorMessage = "Danh mục không được để trống")]
        Guid CategoryId
    );
}
