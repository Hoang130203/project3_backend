using ApiGateway.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Data;

namespace ProfileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupInfoController : ControllerBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly AuthServiceGrpc.AuthServiceGrpcClient _authClient;
        private readonly ILogger<GroupInfoController> _logger;

        public GroupInfoController(
            IProfileRepository profileRepository,
            ILogger<GroupInfoController> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;

            var channel = GrpcChannel.ForAddress("http://authservice:5001");
            _authClient = new AuthServiceGrpc.AuthServiceGrpcClient(channel);
        }

        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupInfoById(Guid groupId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }
                //var permissionRequest = new GroupPermissionRequest
                //{
                //    UserId = userId.ToString(),
                //    GroupId = groupId.ToString(),
                //    Permission = "GroupCommentCreate"
                //};
                // Check if user has permission to view group
                //var permissionResponse = await _authClient.CheckGroupPermissionAsync(permissionRequest);


                //if (!permissionResponse.IsAllowed)
                //{
                //    return Forbid();
                //}

                var groupInfo = await _profileRepository.GetGroupInfoByIdAsync(groupId);
                return Ok(groupInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group info for group {GroupId}", groupId);
                return StatusCode(500, "An error occurred while retrieving group information");
            }
        }


        [HttpPut("{groupId}/groupPicture")]
        public async Task<IActionResult> UpdateGroupPictureUrl(Guid groupId, [FromBody] string pictureUrl)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }
                var permissionRequest = new GroupPermissionRequest
                {
                    UserId = userId.ToString(),
                    GroupId = groupId.ToString(),
                    Permission = "GroupEdit"
                };
                // Check if user has permission to modify group settings
                var permissionResponse = await _authClient.CheckGroupPermissionAsync(permissionRequest);


                if (!permissionResponse.IsAllowed)
                {
                    return Forbid();
                }

                var groupInfo = await _profileRepository.GetGroupInfoByIdAsync(groupId);
                groupInfo.GroupPictureUrl = pictureUrl;
                var updatedGroupInfo = await _profileRepository.StoreGroupInfo(groupInfo);

                _logger.LogInformation(
                    "Group picture updated for group {GroupId} by user {UserId}",
                    groupId,
                    userId
                );

                return Ok(updatedGroupInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group picture for group {GroupId}", groupId);
                return StatusCode(500, "An error occurred while updating group picture");
            }
        }

        [HttpPut("{groupId}/groupBackground")]
        public async Task<IActionResult> UpdateGroupBackground(Guid groupId, [FromBody] string backgroundUrl)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }
                var permissionRequest = new GroupPermissionRequest
                {
                    UserId = userId.ToString(),
                    GroupId = groupId.ToString(),
                    Permission = "GroupEdit"
                };
                var permissionResponse = await _authClient.CheckGroupPermissionAsync(permissionRequest);

                if (!permissionResponse.IsAllowed)
                {
                    return Forbid();
                }

                var groupInfo = await _profileRepository.GetGroupInfoByIdAsync(groupId);
                groupInfo.GroupBackgroundUrl = backgroundUrl;
                var updatedGroupInfo = await _profileRepository.StoreGroupInfo(groupInfo);

                _logger.LogInformation(
                    "Group background updated for group {GroupId} by user {UserId}",
                    groupId,
                    userId
                );

                return Ok(updatedGroupInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group background for group {GroupId}", groupId);
                return StatusCode(500, "An error occurred while updating group background");
            }
        }

        private string? GetCurrentUserId()
        {
            return Request.Headers["X-UserId"].FirstOrDefault();
        }
    }
}
