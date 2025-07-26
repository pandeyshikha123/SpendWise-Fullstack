using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SpendWiseAPI.DTOs
{
    public class ExpenseCreateDTO
    {
        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [JsonIgnore]
        [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
        public string? UserId { get; set; }

        public string? Category { get; set; }

        public string? Notes { get; set; }
    }



    public class ExpenseResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = null!;
        public string Notes { get; set; } = null!;
        public string Type { get; set; } = "Expense";
    }
}
