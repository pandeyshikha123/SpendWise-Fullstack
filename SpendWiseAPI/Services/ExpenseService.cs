using MongoDB.Driver;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Models;
using AutoMapper;
using MongoDB.Bson;
using SpendWiseAPI.Services.Interfaces;
using SpendWiseAPI.Repositories.Interfaces;

namespace SpendWiseAPI.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IMongoCollection<Expense> _expenseCollection;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;



        public ExpenseService(IMongoDatabase mongoDatabase, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IEmailService emailService, IUserRepository userRepository)
        {
            _expenseCollection = mongoDatabase.GetCollection<Expense>("Expenses");
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        }

        public async Task<ExpenseResponseDTO> CreateExpenseAsync(ExpenseCreateDTO dto)
        {
            try
            {
                var userId = dto.UserId; // Use the one passed from controller

                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User ID not found");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                var expense = _mapper.Map<Expense>(dto);
                expense.Type = "Expense";
                expense.UserId = userId;

                await _expenseCollection.InsertOneAsync(expense);

                // Send Email using Razor template Views/EmailTemplates/AddExpense.cshtml
                await _emailService.SendExpenseAddedEmail(user.Email, user.Username, expense.Category, expense.Amount, expense.Date, expense.Description);

                return _mapper.Map<ExpenseResponseDTO>(expense);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating expense: {ex.Message}");
            }
        }


        public async Task<List<ExpenseResponseDTO>> GetUserExpensesAsync(string userId)
        {
            var expenses = await _expenseCollection.Find(e => e.UserId == userId).ToListAsync();
            return _mapper.Map<List<ExpenseResponseDTO>>(expenses);
        }

        public async Task<ExpenseResponseDTO?> GetExpenseByIdAsync(string id)
        {
            var expense = await _expenseCollection.Find(e => e.Id == id).FirstOrDefaultAsync();
            return expense == null ? null : _mapper.Map<ExpenseResponseDTO>(expense);
        }

        public async Task<bool> UpdateExpenseAsync(string id, ExpenseCreateDTO dto)
        {
            try
            {
                var userId = dto.UserId; 

                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User ID not found");

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception("User not found");

                var updatedExpense = _mapper.Map<Expense>(dto);
                updatedExpense.Id = id;
                updatedExpense.Type = "Expense";
                updatedExpense.UserId = userId;

                var result = await _expenseCollection.ReplaceOneAsync(
                    e => e.Id == id && e.UserId == userId, updatedExpense);

                if (result.ModifiedCount > 0)
                {
                    await _emailService.SendExpenseUpdatedEmail(
                        user.Email,
                        user.Username,
                        updatedExpense.Category,
                        updatedExpense.Amount,
                        updatedExpense.Date,
                        updatedExpense.Description
                    );
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating expense: {ex.Message}");
            }
        }


        public async Task<bool> DeleteExpenseAsync(string id)
        {
            var result = await _expenseCollection.DeleteOneAsync(e => e.Id == id);
            return result.DeletedCount > 0;
        }
        public async Task<ExpenseSummaryDTO> GetExpenseSummaryAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is required.");
            }

            // Step 1: Total Summary
            var totalAggregation = await _expenseCollection.Aggregate()
                .Match(e => e.UserId == userId)
                .Group(new BsonDocument
                {
            { "_id", BsonNull.Value },
            { "total", new BsonDocument("$sum", "$Amount") },
            { "count", new BsonDocument("$sum", 1) }
                })
                .FirstOrDefaultAsync();

            var total = totalAggregation?["total"]?.ToDecimal() ?? 0;
            var count = totalAggregation?["count"]?.AsInt32 ?? 0;

            // Step 2: Group by Category
            var categoryResults = await _expenseCollection.Aggregate()
                .Match(e => e.UserId == userId && e.Type == "Expense")
                .Group(new BsonDocument
                {
            { "_id", "$Category" },
            { "total", new BsonDocument("$sum", "$Amount") },
            { "count", new BsonDocument("$sum", 1) }
                })
                .Sort(Builders<BsonDocument>.Sort.Ascending("_id"))
                .ToListAsync();

            var byCategory = categoryResults.Select(c => new ExpenseCategorySummary
            {
                Category = c["_id"]?.AsString ?? "Uncategorized",
                Total = c["total"]?.ToDecimal() ?? 0,
                Count = c["count"]?.AsInt32 ?? 0
            }).ToList();

            return new ExpenseSummaryDTO
            {
                Total = total,
                Count = count,
                ByCategory = byCategory
            };
        }

        public async Task<object> GetUserExpensesFilteredAsync(
        string userId,
        string? category,
        string? type,
        string? period,
        string? search,
        string? sort,
        int page,
        int pageSize)
        {
            var query = _expenseCollection.AsQueryable().Where(e => e.UserId == userId);

            if (!string.IsNullOrEmpty(category) && category != "All")
                query = query.Where(e => e.Category == category);

            if (!string.IsNullOrEmpty(type))
                query = query.Where(e => e.Type == type);

            if (!string.IsNullOrEmpty(period) && period != "All")
            {
                var days = period == "Weekly" ? 7 : 30;
                var fromDate = DateTime.UtcNow.AddDays(-days);
                query = query.Where(e => e.Date >= fromDate);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var lowered = search.ToLower();
                query = query.Where(e =>
                    e.Description.ToLower().Contains(lowered) ||
                    (e.Category != null && e.Category.ToLower().Contains(lowered)) ||
                    e.Amount.ToString().Contains(lowered)
                );
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort switch
                {
                    "date-asc" => query.OrderBy(e => e.Date),
                    "price-asc" => query.OrderBy(e => e.Amount),
                    "price-desc" => query.OrderByDescending(e => e.Amount),
                    _ => query.OrderByDescending(e => e.Date)
                };
            }
            else
            {
                query = query.OrderByDescending(e => e.Date);
            }

            var total = query.Count(); // AsQueryable allows sync Count
            var expenses = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new
            {
                expenses,
                total
            };
        }


    }
}
