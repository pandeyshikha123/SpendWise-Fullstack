namespace SpendWiseAPI.DTOs
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public UserResponseDTO User { get; set; } = null!;
    }
}
