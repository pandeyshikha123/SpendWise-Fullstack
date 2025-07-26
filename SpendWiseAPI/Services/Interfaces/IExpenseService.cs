using SpendWiseAPI.DTOs;

namespace SpendWiseAPI.Services
{
    public interface IExpenseService
    {
        Task<ExpenseResponseDTO> CreateExpenseAsync(ExpenseCreateDTO dto);
        Task<List<ExpenseResponseDTO>> GetUserExpensesAsync(string userId);
        Task<ExpenseResponseDTO?> GetExpenseByIdAsync(string id);
        Task<bool> UpdateExpenseAsync(string id, ExpenseCreateDTO dto);
        Task<bool> DeleteExpenseAsync(string id);
        Task<ExpenseSummaryDTO> GetExpenseSummaryAsync(string userId);
        Task<object> GetUserExpensesFilteredAsync(
            string userId,
            string? category,
            string? type,
            string? period,
            string? search,
            string? sort,
            int page,
            int pageSize
        );



    }
}
