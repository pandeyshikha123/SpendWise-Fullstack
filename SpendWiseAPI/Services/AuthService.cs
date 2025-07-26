using AutoMapper;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Models;
using SpendWiseAPI.Repositories.Interfaces;
using SpendWiseAPI.Services.Interfaces;
using System;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;

    public AuthService(
        IUserRepository userRepository,
        ITokenService tokenService,
        IMapper mapper,
        IEmailService emailService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<LoginResponseDTO?> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshToken(refreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return null;

        var accessToken = _tokenService.CreateAccessToken(user);
        var newRefreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _userRepository.UpdateUserAsync(user);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserResponseDTO>(user)
        };
    }

    public async Task<LoginResponseDTO> SignUpAsync(UserRegisterDTO dto)
    {
        var existing = await _userRepository.GetByEmailAsync(dto.Email.ToLower());
        if (existing != null)
            throw new Exception("User already exists");

        var user = _mapper.Map<User>(dto);
        user.Email = user.Email.ToLower();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        await _userRepository.AddUserAsync(user);
        await _emailService.SendWelcomeEmail(user.Email, user.Username);

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateUserAsync(user);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserResponseDTO>(user)
        };
    }

    public async Task<LoginResponseDTO> SignInAsync(SignInDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await _userRepository.UpdateUserAsync(user);

        return new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserResponseDTO>(user)
        };
    }

    public async Task<UserResponseDTO?> GetUserAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserResponseDTO>(user) : null;
    }

    public async Task<UserResponseDTO?> UpdateProfilePictureAsync(string userId, string imageUrl)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.ProfilePicture = imageUrl;
        await _userRepository.UpdateUserAsync(user);

        return _mapper.Map<UserResponseDTO>(user);
    }
}
