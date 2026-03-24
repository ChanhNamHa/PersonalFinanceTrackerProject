using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;

namespace PersonalFinanceTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpPost]
        public async Task<ActionResult<BudgetResponse>> Create(CreateBudgetRequest request)
        {
            var userId = GetUserId();
            var result = await _budgetService.CreateBudgetAsync(userId, request);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetAll()
        {
            var userId = GetUserId();
            var result = await _budgetService.GetUserBudgetsAsync(userId);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var success = await _budgetService.DeleteBudgetAsync(userId, id);

            if (!success)
                return NotFound("Không tìm thấy ngân sách để xóa.");

            return NoContent(); // Trả về 204
        }

        private Guid GetUserId()
        {
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }
    }
}
