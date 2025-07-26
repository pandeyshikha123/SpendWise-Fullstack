using SpendWiseAPI.DTOs;

public interface IAuthService
{
    Task<LoginResponseDTO?> RefreshTokenAsync(string refreshToken);
    Task<LoginResponseDTO> SignUpAsync(UserRegisterDTO dto);
    Task<LoginResponseDTO> SignInAsync(SignInDto dto);
    Task<UserResponseDTO?> GetUserAsync(string userId);
    Task<UserResponseDTO?> UpdateProfilePictureAsync(string userId, string imageUrl);
}
