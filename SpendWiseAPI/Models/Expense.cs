using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SpendWiseAPI.Models
{
    public class Expense
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        public string? Category { get; set; }

        [Required]
        [RegularExpression("Income|Expense")]
        public string Type { get; set; } = "Expense";

        // 🔷 Optional: You can add this if you want future filtering/sorting
        public string? Notes { get; set; }   // 🔹 NEW FIELD (optional)
    }
}
