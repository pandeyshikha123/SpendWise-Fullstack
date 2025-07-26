using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWiseAPI.DTOs;
using SpendWiseAPI.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    public AuthController(IAuthService authService, IMapper mapper, IImageService imageService)
    {
        _authService = authService;
        _mapper = mapper;
        _imageService = imageService;
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var response = await _authService.RefreshTokenAsync(dto.RefreshToken);
        if (response == null)
            return Unauthorized("Invalid refresh token.");

        return Ok(response);
    }

    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(UserRegisterDTO dto)
    {
        try
        {
            var response = await _authService.SignUpAsync(dto);
            return Ok(response);
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
            var response = await _authService.SignInAsync(dto);
            return Ok(response);
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

        var user = await _authService.GetUserAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }

    [Authorize]
    [HttpPost("profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
    {
        if (profilePicture == null || profilePicture.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var imageUrl = await _imageService.UploadImageAsync(profilePicture);
        if (string.IsNullOrEmpty(imageUrl))
            return StatusCode(500, new { message = "Image upload to Cloudinary failed" });

        var user = await _authService.UpdateProfilePictureAsync(userId, imageUrl);
        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(new { message = "Profile picture updated", user });
    }
}
