using Microsoft.AspNetCore.Mvc;
using ProfileService.Data;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(
            IProfileRepository profileRepository,
            ILogger<UserProfileController> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfileById(Guid userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return Unauthorized();
            }


            var profile = await _profileRepository.GetUserProfileByIdAsync(userId);
            return Ok(profile);
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] string avatarUrl)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _profileRepository.GetUserProfileByIdAsync(Guid.Parse(userId));
            profile.AvatarUrl = avatarUrl;
            await _profileRepository.StoreProfile(profile);
            return Ok(profile);
        }
        [HttpPut("background")]
        public async Task<IActionResult> UpdateBackground([FromBody] string backgroundUrl)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var profile = await _profileRepository.GetUserProfileByIdAsync(Guid.Parse(userId));
            profile.ProfileBackgroundUrl = backgroundUrl;
            await _profileRepository.StoreProfile(profile);
            return Ok(profile);
        }

        [HttpGet("userProfiles")]
        public async Task<IActionResult> GetUserProfiles()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }

            var userType = GetCurrentUserType();

            if (userType != "SystemAdmin")
            {
                return Forbid();
            }

            var profiles = await _profileRepository.GetUserProfilesAsync();
            return Ok(profiles);
        }

        private string? GetCurrentUserId()
        {
            return Request.Headers["X-UserId"].FirstOrDefault();
        }

        private string? GetCurrentUserType()
        {
            return Request.Headers["X-UserType"].FirstOrDefault();
        }
    }
}
