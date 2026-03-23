using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using System.Security.Claims;

namespace PersonalFinanceTracker.API.Controllers
{
    [Authorize] // Bắt buộc phải có JWT Token
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDTO>> Create(CreateTransactionRequest request)
        {
            // Lấy UserId từ Claims của JWT Token đã được xác thực
            var userId = GetUserId();

            var result = await _transactionService.CreateTransactionAsync(userId, request);

            // Trả về 201 Created kèm theo dữ liệu vừa tạo
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDTO>>> GetAll()
        {
            var userId = GetUserId();
            var transactions = await _transactionService.GetUserTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDTO>> GetById(Guid id)
        {
            // 1. Lấy UserId từ Token để đảm bảo tính riêng tư
            var userId = GetUserId();

            // 2. Gọi Service để lấy dữ liệu
            var transaction = await _transactionService.GetTransactionByIdAsync(userId, id);

            // 3. Kiểm tra xem có dữ liệu không
            if (transaction == null)
            {
                return NotFound("Giao dịch không tồn tại hoặc bạn không có quyền truy cập.");
            }

            return Ok(transaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var success = await _transactionService.DeleteTransactionAsync(userId, id);

            if (!success) return NotFound("Giao dịch không tồn tại hoặc bạn không có quyền xóa.");

            return NoContent(); // 204 No Content
        }

        // Helper method để lấy UserId từ Token một cách an toàn
        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng trong Token.");

            return Guid.Parse(userIdClaim.Value);
        }
    }
}