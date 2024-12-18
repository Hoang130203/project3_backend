using Grpc.Core;
using ProfileService.Data;
using ProfileService.Models;
using ProfileService.Proto;
using SocialAppObjects;

namespace ProfileService.Services
{
    public class ProfileGrpcService : ProfileServiceGrpc.ProfileServiceGrpcBase
    {
        private readonly IProfileRepository _profileRepository;
        private readonly ILogger<ProfileGrpcService> _logger;

        public ProfileGrpcService(
            IProfileRepository profileRepository,
            ILogger<ProfileGrpcService> logger)
        {
            _profileRepository = profileRepository;
            _logger = logger;
        }

        public override async Task<SubInfoResponse> GetSubInfo(SubInfoRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);
                var subInfo = new SubInfo();
                try
                {
                    subInfo = await _profileRepository.GetUserNameAndAvatarByIdAsync(id);
                }
                catch (Exception ex)
                {
                    subInfo = await _profileRepository.GetGroupNameAndAvatarByIdAsync(id);
                }
 

                if (subInfo == null)
                {
                    return new SubInfoResponse
                    {
                        Id = subInfo.Id.ToString(),
                        Name = "",
                        AvatarUrl = ""
                    };
                }

                return new SubInfoResponse
                {
                    Id = subInfo.Id.ToString(),
                    Name = subInfo.Name,
                    AvatarUrl = subInfo.AvatarUrl 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SubInfo for ID: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<MultipleSubInfoResponse> GetMultipleSubInfo(MultipleSubInfoRequest request, ServerCallContext context)
        {
            var response = new MultipleSubInfoResponse();
            foreach (var id in request.Ids)
            {
                try
                {
                    var subInfoRequest = new SubInfoRequest { Id = id };
                    var subInfo = await GetSubInfo(subInfoRequest, context);
                    response.SubInfos.Add(subInfo);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
                {
                    // Skip not found entities
                    continue;
                }
            }
            return response;
        }

        public override async Task<UserProfileResponse> GetUserProfile(ProfileRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var profile = await _profileRepository.GetUserProfileByIdAsync(userId);

                return MapToUserProfileResponse(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for ID: {UserId}", request.UserId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<UserProfileResponse> CreateUserProfile(CreateProfileRequest request, ServerCallContext context)
        {
            try
            {
                var profile = new UserProfile
                {
                    Id = Guid.Parse(request.Id),
                    Email = request.Email,
                    FullName = request.FullName,
                    AvatarUrl = request.AvatarUrl,
                    ProfileBackgroundUrl = request.ProfileBackgroundUrl,
                    Bio = request.Bio,
                    IsMale = request.IsMale,
                    Location = request.Location,
                    Website = request.Website,
                    Phone = request.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                var created = await _profileRepository.StoreProfile(profile);
                return MapToUserProfileResponse(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<UserProfileResponse> UpdateUserProfile(UpdateProfileRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.Id);
                var existingProfile = await _profileRepository.GetUserProfileByIdAsync(userId);

                if (request.Email != null) existingProfile.Email = request.Email;
                if (request.FullName != null) existingProfile.FullName = request.FullName;
                if (request.AvatarUrl != null) existingProfile.AvatarUrl = request.AvatarUrl;
                if (request.ProfileBackgroundUrl != null) existingProfile.ProfileBackgroundUrl = request.ProfileBackgroundUrl;
                if (request.Bio != null) existingProfile.Bio = request.Bio;
                if (request.IsMale != null) existingProfile.IsMale = request.IsMale;
                if (request.Location != null) existingProfile.Location = request.Location;
                if (request.Website != null) existingProfile.Website = request.Website;
                if (request.Phone != null) existingProfile.Phone = request.Phone;

                existingProfile.UpdatedAt = DateTime.UtcNow;

                var updated = await _profileRepository.StoreProfile(existingProfile);
                return MapToUserProfileResponse(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile for ID: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<DeleteResponse> DeleteUserProfile(ProfileRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var success = await _profileRepository.DeleteProfile(userId);

                return new DeleteResponse
                {
                    Success = success,
                    Message = success ? "Profile deleted successfully" : "Failed to delete profile"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile for ID: {UserId}", request.UserId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<GroupInfoResponse> GetGroupInfo(GroupInfoRequest request, ServerCallContext context)
        {
            try
            {
                var groupId = Guid.Parse(request.GroupId);
                var groupInfo = await _profileRepository.GetGroupInfoByIdAsync(groupId);

                return MapToGroupInfoResponse(groupInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group info for ID: {GroupId}", request.GroupId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<GroupInfoResponse> UpdateGroupInfo(UpdateGroupInfoRequest request, ServerCallContext context)
        {
            try
            {
                var groupId = Guid.Parse(request.Id);
                var existingGroup = await _profileRepository.GetGroupInfoByIdAsync(groupId);

                if (request.Name != null) existingGroup.Name = request.Name;
                if (request.Description != null) existingGroup.Description = request.Description;
                if (request.GroupPictureUrl != null) existingGroup.GroupPictureUrl = request.GroupPictureUrl;
                if (request.GroupBackgroundUrl != null) existingGroup.GroupBackgroundUrl = request.GroupBackgroundUrl;
                if (request.IsLocked != null) existingGroup.IsLocked = request.IsLocked;

                existingGroup.UpdatedAt = DateTime.UtcNow;

                var updated = await _profileRepository.StoreGroupInfo(existingGroup);
                return MapToGroupInfoResponse(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group info for ID: {Id}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<DeleteResponse> DeleteGroupInfo(GroupInfoRequest request, ServerCallContext context)
        {
            try
            {
                var groupId = Guid.Parse(request.GroupId);
                var success = await _profileRepository.DeleteGroupInfo(groupId);

                return new DeleteResponse
                {
                    Success = success,
                    Message = success ? "Group info deleted successfully" : "Failed to delete group info"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group info for ID: {GroupId}", request.GroupId);
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        private static UserProfileResponse MapToUserProfileResponse(UserProfile profile)
        {
            return new UserProfileResponse
            {
                Id = profile.Id.ToString(),
                Email = profile.Email,
                FullName = profile.FullName,
                AvatarUrl = profile.AvatarUrl ?? "",
                ProfileBackgroundUrl = profile.ProfileBackgroundUrl,
                Bio = profile.Bio ?? "",
                IsMale = profile.IsMale,
                Location = profile.Location,
                Website = profile.Website,
                Phone = profile.Phone,
                MaritalStatus = profile.maritalStatus.ToString(),
                CreatedAt = profile.CreatedAt.ToString("O"),
                UpdatedAt = profile.UpdatedAt?.ToString("O") ?? ""
            };
        }

        private static GroupInfoResponse MapToGroupInfoResponse(GroupInfo group)
        {
            return new GroupInfoResponse
            {
                Id = group.Id.ToString(),
                Name = group.Name,
                Description = group.Description,
                OwnerId = group.OwnerId.ToString(),
                OwnerName = group.OwnerName,
                GroupPictureUrl = group.GroupPictureUrl,
                GroupBackgroundUrl = group.GroupBackgroundUrl,
                CreatedAt = group.CreatedAt.ToString("O"),
                UpdatedAt = group.UpdatedAt?.ToString("O") ?? "",
                Visibility = group.Visibility.ToString(),
                IsDeleted = group.IsDeleted,
                IsLocked = group.IsLocked
            };
        }
    }
}
