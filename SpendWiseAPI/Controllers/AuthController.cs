using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Repositories;
using SpendWiseAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    private readonly UserRepository _userRepo;
    private readonly IMapper _mapper;
    private readonly TokenService _tokenService;
    private readonly IImageService _imageService;


    public AuthController(AuthService service, IMapper mapper, IImageService imageService)
    {
        _service = service;
        _mapper = mapper;
        _imageService = imageService;
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        // Validate refresh token from DB or session
        // If valid, generate new access token
        var user = await _userRepo.GetUserByRefreshToken(dto.RefreshToken);
        if (user == null) return Unauthorized("Invalid refresh token.");

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = _tokenService.CreateRefreshToken();

        // Save new refresh token to DB/session if needed

        return Ok(new LoginResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserResponseDTO>(user)
            // User = _mapper.Map<UserLoginDTO>(user)
        });
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(UserRegisterDTO dto)
    {
        try
        {
            var (accessToken, refreshToken, user) = await _service.SignUpAsync(dto);
            return Ok(new { accessToken, refreshToken, user });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                stackTrace = ex.StackTrace,
                inner = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInDto dto)
    {
        try
        {
            var (accessToken, refreshToken, user) = await _service.SignInAsync(dto);
            return Ok(new { accessToken, refreshToken, user });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID not found in token");

        var user = await _service.GetUserAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }



[Authorize]
[HttpPost("profile-picture")]
public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
{
    if (profilePicture == null || profilePicture.Length == 0)
        return BadRequest(new { message = "No file uploaded" });

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId == null)
        return Unauthorized();

    // Upload to Cloudinary
    var imageUrl = await _imageService.UploadImageAsync(profilePicture);
    if (string.IsNullOrEmpty(imageUrl))
        return StatusCode(500, new { message = "Image upload to Cloudinary failed" });

    // Update user record
    var user = await _service.UpdateProfilePictureAsync(userId, imageUrl);
    if (user == null)
        return NotFound(new { message = "User not found" });

    return Ok(new { message = "Profile picture updated", user });
}

}
