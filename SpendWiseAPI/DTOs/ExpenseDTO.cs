namespace SpendWiseAPI.DTOs
{
    public class ExpenseCreateDTO
    {
        public string Description { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

         public string UserId { get; set; } = null!;

        public string? Category { get; set; }

        public string Type { get; set; } = "Expense";

        public string? Notes { get; set; }
    }

    public class ExpenseResponseDTO
    {
        public string Id { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } = "Expense";

        public string? Category { get; set; }

        public string? Notes { get; set; }
    }
}
