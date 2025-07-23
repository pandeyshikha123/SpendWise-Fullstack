namespace SpendWiseAPI.DTOs
{
    public class UserRegisterDTO
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class UserLoginDTO
    {
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }

    public class UserResponseDTO
    {
        public string Id { get; set; } = null!;

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string ProfilePicture { get; set; } = string.Empty;

        public bool EmailNotifications { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
