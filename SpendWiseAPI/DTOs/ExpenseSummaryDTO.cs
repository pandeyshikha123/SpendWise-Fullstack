namespace SpendWiseAPI.DTOs
{
    public class ExpenseSummaryDTO
    {
        public decimal Total { get; set; }
        public int Count { get; set; }
        public List<ExpenseCategorySummary> ByCategory { get; set; } = new();
    }

    public class ExpenseCategorySummary
    {
        public string Category { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
