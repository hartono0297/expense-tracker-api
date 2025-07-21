using ExpenseTracker.Data;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Models;
using ExpenseTracker.Models.Responses;
using ExpenseTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet] 
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var user = await _userService.GetUserByIdAsync(userId);

                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "User found successfully."));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ApiResponse<UserResponseDto>.Fail(ex.Message));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while getting user.");
                return BadRequest(ApiResponse<UserResponseDto>.Fail("Database error occured"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> CreateUser(RegisterRequestDto regis)
        {         
            try
            {
                await _userService.CreateUserAsync(regis);
                return Ok(ApiResponse<string>.SuccessResponse("", "User created successfully."));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while creating user.");
                return BadRequest(ApiResponse<string>.Fail("Database error occured"));
            }
        }

        [Authorize]
        [HttpPut] 
        public async Task<ActionResult<ApiResponse<string>>> UpdateUser(UserUpdateDto user)
        {         
            try
            {
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                if (userIdClaim.ToString() == null || !int.TryParse(userIdClaim.ToString(), out var userId))
                {
                    return Unauthorized(ApiResponse<string>.Fail("Unauthorized access."));
                }

                await _userService.UpdateUserAsync(userId, user);
                return Ok(ApiResponse<string>.SuccessResponse("", "User updated successfully."));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed: {Message}", ex.Message);                
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogWarning(dbEx, "Database error occurred while updating user.");                
                return BadRequest(ApiResponse<string>.Fail("Database error occured"));
            }
        }
    }
}
