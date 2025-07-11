using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentRegistration.Api.Contracts;
using StudentRegistration.Api.Services;
using StudentRegistration.Application.Services;
using StudentRegistration.Domain.Interfaces;
using StudentRegistration.Domain.Entities;

namespace StudentRegistration.Api.Controllers;

/// <summary>
/// Controller xử lý authentication
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenService refreshTokenService,
        ILogger<AuthController> logger)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Đăng nhập và nhận JWT token + Refresh token
    /// </summary>
    /// <param name="request">Thông tin đăng nhập</param>
    /// <returns>JWT token và refresh token nếu đăng nhập thành công</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Username và password không được để trống" });
            }

            // Validate credentials
            var user = await _userRepository.ValidateCredentialsAsync(request.Username, request.Password);
            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
                return Unauthorized(new { message = "Username hoặc password không đúng" });
            }

            // Mapping từ User (Domain) sang UserInfo (DTO)
            var userInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            // Generate JWT token
            var accessToken = _jwtTokenGenerator.GenerateToken(userInfo);
            
            // Generate refresh token
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = 300, // 5 phút (ngắn hơn cho bảo mật)
                RefreshTokenExpiresIn = 604800, // 7 ngày
                TokenType = "Bearer"
            };

            _logger.LogInformation("Login successful for user: {Username} with role: {Role}", user.Username, user.Role);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user: {Username}", request.Username);
            return StatusCode(500, new { message = "Lỗi server trong quá trình đăng nhập" });
        }
    }

    /// <summary>
    /// Refresh access token bằng refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>Access token mới</returns>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            _logger.LogInformation("Refresh token attempt");

            // Validate refresh token
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest(new { message = "Refresh token không được để trống" });
            }

            // Validate refresh token
            var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken);
            if (!isValid)
            {
                _logger.LogWarning("Invalid refresh token attempt");
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn" });
            }

            // Get refresh token details
            var refreshTokenEntity = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshTokenEntity == null)
            {
                return Unauthorized(new { message = "Refresh token không tồn tại" });
            }

            // Get user information
            var user = await _userRepository.GetByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
            {
                return Unauthorized(new { message = "User không tồn tại" });
            }

            // Mapping từ User (Domain) sang UserInfo (DTO)
            var userInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            // Generate new access token
            var newAccessToken = _jwtTokenGenerator.GenerateToken(userInfo);
            
            // Revoke old refresh token for security (optional)
            await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, "Refresh");
            
            // Generate new refresh token
            var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);
            
            var response = new RefreshTokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = 300, // 5 phút
                TokenType = "Bearer"
            };

            _logger.LogInformation("Token refresh successful for user: {Username}", user.Username);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "Lỗi server trong quá trình refresh token" });
        }
    }

    /// <summary>
    /// Lấy thông tin user hiện tại
    /// </summary>
    /// <returns>Thông tin user</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Token không hợp lệ" });
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Không tìm thấy user" });
            }

            // Mapping sang DTO
            var userInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            return Ok(userInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, new { message = "Lỗi server" });
        }
    }

    /// <summary>
    /// Validate token
    /// </summary>
    /// <returns>Status OK nếu token hợp lệ</returns>
    [HttpGet("validate")]
    [Authorize]
    public IActionResult ValidateToken()
    {
        return Ok(new { message = "Token hợp lệ" });
    }

    /// <summary>
    /// Logout - revoke refresh token
    /// </summary>
    /// <param name="request">Refresh token cần revoke</param>
    /// <returns>Status OK</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken, "Logout");
            }

            return Ok(new { message = "Đăng xuất thành công" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "Lỗi server trong quá trình đăng xuất" });
        }
    }
}
