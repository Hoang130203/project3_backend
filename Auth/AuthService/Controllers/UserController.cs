using AuthService.Domain.Entities.Users;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using AuthService.Domain.Enums;
using AuthService.Domain.Entities.Groups;
using AuthService.Application.DTOs.User;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRelationshipRepository _relationshipRepository;
        private readonly IUserRepository _userService;
        private readonly ILogger<UserController> _logger;
        private readonly IGroupRepository _groupRepository;
        public UserController(IUserRelationshipRepository relationshipRepository,
            IUserRepository userRepository,
            ILogger<UserController> logger,
            IGroupRepository groupRepository)
        {
            _relationshipRepository = relationshipRepository;
            _userService = userRepository;
            _logger = logger;
            _groupRepository = groupRepository;
        }

        [HttpPost("send-friend-request/{targetUserId}")]
        public async Task<IActionResult> SendFriendRequest(Guid targetUserId)
        {
            var sourceUserId = GetCurrentUserId(); // Get from authenticated user

            // Check if users exist and validate request
            var sourceUser = await _userService.GetUserByIdAsync(sourceUserId);
            var targetUser = await _userService.GetUserByIdAsync(targetUserId);

            if (sourceUser == null || targetUser == null)
            {
                return NotFound("User not found");
            }

            // Check if request already exists
            var existingRelationship = await _relationshipRepository.GetUserConnectionByIdAsync(sourceUserId, targetUserId);
            if (existingRelationship != null)
            {
                return BadRequest("Friend request already exists");
            }

            // Create new relationship
            var relationship = new UserRelationship
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId,
                Status = ConnectionStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _relationshipRepository.CreateUserConnectionAsync(relationship);

            return Ok();
        }

        [HttpPost("accept-friend-request/{sourceUserId}")]
        public async Task<IActionResult> AcceptFriendRequest(Guid sourceUserId)
        {
            var targetUserId = GetCurrentUserId();

            var relationship = await _relationshipRepository.GetUserConnectionByIdAsync(sourceUserId, targetUserId);
            if (relationship == null || relationship.Status != ConnectionStatus.Pending)
            {
                return BadRequest("No pending friend request found");
            }

            relationship.Status = ConnectionStatus.Connected;
            relationship.UpdatedAt = DateTime.UtcNow;

            await _relationshipRepository.UpdateUserConnectionAsync(relationship);

            return Ok();
        }

        [HttpPost("reject-friend-request/{sourceUserId}")]
        public async Task<IActionResult> RejectFriendRequest(Guid sourceUserId)
        {
            var targetUserId = GetCurrentUserId();

            var relationship = await _relationshipRepository.GetUserConnectionByIdAsync(sourceUserId, targetUserId);
            //if (relationship == null || relationship.Status != ConnectionStatus.Pending)
            //{
            //    return BadRequest("No pending friend request found");
            //}

            await _relationshipRepository.DeleteUserConnectionAsync(relationship.Id);

            return Ok();
        }

        [HttpGet("friend-requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            var userId = GetCurrentUserId();
            var relationships = await _relationshipRepository.GetUserConnectionsAsync(userId);

            var pendingRequests = relationships.Where(r =>
                r.TargetUserId == userId &&
                r.Status == ConnectionStatus.Pending);

            return Ok(pendingRequests);
        }
        [HttpGet("friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = GetCurrentUserId();
            var relationships = await _userService.GetFriendsAsync(userId);
            return Ok(relationships);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var userId = GetCurrentUserId();
            var groups = await _userService.GetGroupsAsync(userId);
            return Ok(groups);
        }
        [HttpGet("createdGroups")]
        public async Task<ActionResult<IEnumerable<Group>>> GetCreatedGroups()
        {
            try
            {
                // Get user ID from token (added by gateway)
                var userIdString = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();

                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                {
                    return Unauthorized();
                }

                var groups = await _groupRepository.GetGroupsByCreatorAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "An error occurred while retrieving created groups" });
            }
        }
        [HttpGet("suggestions")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetFriendSuggestions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var skip = (page - 1) * pageSize;

                    var suggestions = await _userService.GetFriendSuggestionsAsync(userId, skip, pageSize);

                    // Map to DTO to avoid sending sensitive information
                    var suggestionDtos = suggestions.Select(user => new UserSuggestionDto
                    {
                        UserId = user.Id,
                        Username = user.Username,
                        FullName = user.Profile?.FullName ?? "",
                        AvatarUrl = user.Profile?.ProfilePictureUrl ?? "",
                        Bio = user.Profile?.Bio ?? "",
                        Location = user.Profile?.Location ?? ""
                    });

                    return Ok(new
                    {
                        Page = page,
                        PageSize = pageSize,
                        Suggestions = suggestionDtos
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting friend suggestions for user {UserId}", GetCurrentUserId());
                    return BadRequest(new { Error = "Failed to get friend suggestions" });
                }
            }
        private Guid GetCurrentUserId()
        {
            var userIdString = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User ID not found in request headers");
            }
            return Guid.Parse(userIdString);
        }
    }
}
