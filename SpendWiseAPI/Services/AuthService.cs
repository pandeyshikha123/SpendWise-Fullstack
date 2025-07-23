using AutoMapper;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Helpers;
using SpendWiseAPI.Models;
using SpendWiseAPI.Repositories;
using SpendWiseAPI.Services.Interfaces;

public class AuthService
{
    private readonly UserRepository _repo;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    // private readonly TokenService _tokenService;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;


    public AuthService(UserRepository repo, IMapper mapper, IConfiguration config, ITokenService tokenService, IEmailService emailService)
    {
        _repo = repo;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _config = config;
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<(string accessToken, string refreshToken, UserResponseDTO user)> SignUpAsync(UserRegisterDTO dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email.ToLower());
        if (existing != null)
            throw new Exception("User already exists");

        var user = _mapper.Map<User>(dto);
        user.Email = user.Email.ToLower();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);


        await _repo.AddUserAsync(user);
        await _emailService.SendWelcomeEmail(user.Email, user.Username);

        // await EmailService.SendWelcomeEmail(user.Email, user.Username); // mocked

        // var token = JwtHelper.GenerateJwtToken(user.Id, _config["Jwt:Key"]);
        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        // Optionally store the refresh token in DB (recommended)
        // await _repo.UpdateRefreshTokenAsync(user.Id, refreshToken);


        var userDto = _mapper.Map<UserResponseDTO>(user);
        // return (token, userDto);

        return (accessToken, refreshToken, userDto);



    }

    public async Task<(string accessToken, string refreshToken, UserResponseDTO user)> SignInAsync(SignInDto dto)
    {
        var user = await _repo.GetByEmailAsync(dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials");

        // var token = JwtHelper.GenerateJwtToken(user.Id, _config["Jwt:Key"]);
        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        var userDto = _mapper.Map<UserResponseDTO>(user);
        // return (token, userDto);
        return (accessToken, refreshToken, userDto);

    }

    public async Task<UserResponseDTO?> GetUserAsync(string userId)
    {
        var user = await _repo.GetByIdAsync(userId);
        return user != null ? _mapper.Map<UserResponseDTO>(user) : null;
    }

   public async Task<UserResponseDTO?> UpdateProfilePictureAsync(string userId, string filePath)
{
    var user = await _repo.GetByIdAsync(userId);
    if (user == null) return null;

    user.ProfilePicture = filePath;

    await _repo.UpdateAsync(user); // Call a method to replace the document in DB

    return _mapper.Map<UserResponseDTO>(user);
}

}
