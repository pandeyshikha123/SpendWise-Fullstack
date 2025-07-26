using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Services;
using System.Security.Claims;

namespace SpendWiseAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // Helper: Get UserId from JWT claims
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new UnauthorizedAccessException("UserId not found in token");
        }

        // POST: /api/Expense
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExpenseCreateDTO dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                // var userId = User.FindFirst("nameid")?.Value; // <-- FIXED
                Console.WriteLine("Extracted UserId from Claims: " + userId);


            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found");

            dto.UserId = userId;

            var result = await _expenseService.CreateExpenseAsync(dto);
            return Ok(result);
        }
        

        // GET: /api/Expense9(Filter for user)
        [HttpGet]
        public async Task<IActionResult> GetUserExpenses(
            [FromQuery] string? category,
            [FromQuery] string? type,
            [FromQuery] string? period,
            [FromQuery] string? search,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            try
            {
                var userId = GetUserId();

                var result = await _expenseService.GetUserExpensesFilteredAsync(
                    userId, category, type, period, search, sort, page, pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get expenses error: " + ex.Message);
                return StatusCode(500, new { message = "Server error" });
            }
        }


        // GET: /api/Expense/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            return expense == null ? NotFound() : Ok(expense);
        }

        // PUT: /api/Expense/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] ExpenseCreateDTO dto)
        {
            dto.UserId = GetUserId(); // Set UserId from JWT
            var updated = await _expenseService.UpdateExpenseAsync(id, dto);
            return updated ? Ok() : NotFound();
        }

        // DELETE: /api/Expense/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _expenseService.DeleteExpenseAsync(id);
            return deleted ? Ok() : NotFound();
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var userId = GetUserId(); // From JWT
                var summary = await _expenseService.GetExpenseSummaryAsync(userId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get expense summary error: " + ex.Message);
                return StatusCode(500, new { message = "Server error" });
            }
        }

    }
}
