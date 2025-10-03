using ExpenseTracker.Data;
using ExpenseTracker.DTOs.ExpenseDtos;
using ExpenseTracker.DTOs.RegisterDtos;
using ExpenseTracker.DTOs.UserDtos;
using ExpenseTracker.Extensions;
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
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(CancellationToken cancellationToken = default)
        {
            if(!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<UserResponseDto>.Fail("User is not authorized"));

            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user, "User found successfully."));
        }

        [Authorize]
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(int userId, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var currentUserId))
                return Unauthorized(ApiResponse<UserResponseDto>.Fail("User is not authorized"));

            // 🚨 if not admin and not the same user → block
            if (!User.IsInRole("Admin") && currentUserId != userId)
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<UserResponseDto>.Fail("Only Admin Allowed or Self Access"));

            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);

            if (user == null)
                return NotFound(ApiResponse<UserResponseDto>.Fail("User not found"));

            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(user));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> CreateUser([FromBody] RegisterRequestDto regis, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RegisterResponseDto>.Fail("Invalid request data"));
            }

            var user =  await _userService.CreateUserAsync(regis, cancellationToken);

            return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, ApiResponse<RegisterResponseDto>.SuccessResponse(user, "User created successfully."));
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUser(UserUpdateDto user, CancellationToken cancellationToken = default)
        {
            if (!User.TryGetUserId(out var userId))
                return Unauthorized(ApiResponse<UserResponseDto>.Fail("User is not authorized"));

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserResponseDto>.Fail("Invalid request data"));
            }

            var result = await _userService.UpdateUserAsync(userId, user, cancellationToken);
                return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User updated successfully."));
        }
    }
}
