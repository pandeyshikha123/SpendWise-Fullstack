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

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;

        public string? Category { get; set; }

        public string? Notes { get; set; }

        [Required]
        public string Type { get; set; } = "Expense"; // always set to Expense
    }
}
