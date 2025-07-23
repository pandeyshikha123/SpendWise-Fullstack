namespace SpendWiseAPI.DTOs
{
    public class CategoryDTO
    {
        public string Name { get; set; } = null!;
    }

    public class CategoryResponseDTO
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
