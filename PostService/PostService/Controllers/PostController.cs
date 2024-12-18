using ApiGateway.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PostService.Interfaces;
using PostService.Models;
using PostService.Models.Request;
using PostService.Services;
using SocialAppObjects.Enums;
using System.IO.Pipelines;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IMediaService _mediaService;
        private readonly IAnalyticsService _analyticsService;
        private readonly AuthServiceGrpc.AuthServiceGrpcClient _authClient;
        private readonly ILogger<PostsController> _logger;

        public PostsController(
             IPostRepository postRepository,
             IMediaService mediaService,
             IAnalyticsService analyticsService,
             ILogger<PostsController> logger)
        {
            _postRepository = postRepository;
            _mediaService = mediaService;
            _analyticsService = analyticsService;
            _logger = logger;

            // Initialize gRPC client
            var channel = GrpcChannel.ForAddress("http://authservice:5001");
            _authClient = new AuthServiceGrpc.AuthServiceGrpcClient(channel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(string id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Track view
            await _analyticsService.TrackViewAsync(id, GetCurrentUserId());

            return Ok(post);
        }

        //[HttpGet("feed")]
        //public async Task<ActionResult<IEnumerable<Post>>> GetFeed(
        //    [FromQuery] int skip = 0,
        //    [FromQuery] int take = 20)
        //{
        //    var userId = GetCurrentUserId();
        //    var posts = await _postRepository.GetUserFeedAsync(userId, skip, take);
        //    return Ok(posts);
        //}

        [HttpPost]
        public async Task<ActionResult<string>> CreatePost([FromBody] CreatePostRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userType = GetCurrentUserType();

                // Check if user has permission to create post based on UserType
                if (userType == "Guest")
                {
                    return Forbid("Guest users cannot create posts");
                }

                var post = new Post
                {
                    Title = request.Title,
                    Content = request.Content,
                    Author = new User
                    {
                        UserId = userId,
                        Name = GetCurrentUserName()
                    },
                    MediaItems = request.MediaItems?.Select(m => new MediaItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContentType = m.ContentType,
                        Url = m.Url,
                        Size = m.Size,
                        Type = m.Type,
                        ThumbnailUrl = m.Url
                    }).ToList() ?? new List<MediaItem>(),
                    Visibility = request.VisibilitySettings ?? new VisibilitySettings
                    {
                        Type = VisibilityType.Public,
                        AllowComments = true,
                        AllowReactions = true,
                        AllowSharing = true
                    }
                };

                var postId = await _postRepository.CreateAsync(post);
                await _analyticsService.TrackViewAsync(postId, userId);

                return Ok(new { Id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("group/{groupId}")]
        public async Task<ActionResult<string>> CreateGroupPost(Guid groupId, [FromBody] CreatePostRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userType = GetCurrentUserType();

                // Check permission through gRPC
                var permissionRequest = new GroupPermissionRequest
                {
                    UserId = userId.ToString(),
                    GroupId = groupId.ToString(),
                    Permission = "GroupPostCreate"
                };

                var permissionResponse = await _authClient.CheckGroupPermissionAsync(permissionRequest);
                if (!permissionResponse.IsAllowed)
                {
                    return Forbid("User does not have permission to create posts in this group");
                }

                var post = new Post
                {
                    Title = request.Title,
                    Content = request.Content,
                    Group = new Group
                    {
                        Id = groupId
                    },
                    Author = new User
                    {
                        UserId = userId,
                        Name = GetCurrentUserName()
                    },
                    MediaItems = request.MediaItems?.Select(m => new MediaItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContentType = m.ContentType,
                        Url = m.Url,
                        Size = m.Size,
                        Type = m.Type,
                        ThumbnailUrl = m.Url
                    }).ToList() ?? new List<MediaItem>(),
                    Visibility = request.VisibilitySettings ?? new VisibilitySettings
                    {
                        Type = VisibilityType.Public,
                        AllowComments = true,
                        AllowReactions = true,
                        AllowSharing = true
                    }
                };

                var postId = await _postRepository.CreateAsync(post);
                await _analyticsService.TrackViewAsync(postId, userId);

                return Ok(new { Id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group post");
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("{postId}/media")]
        public async Task<ActionResult<MediaItem>> UploadMedia(
            string postId,
            IFormFile file)
        {
            //using var stream = file.OpenReadStream();
            var mediaItem = await _mediaService.UploadMediaAsync(file);

            var update = Builders<Post>.Update.Push(p => p.MediaItems, mediaItem);
            await _postRepository.UpdateMediaAsync(postId, update);

            return Ok(mediaItem);
        }

        [HttpGet("{postId}/analytics")]
        public async Task<ActionResult<EngagementReport>> GetAnalytics(string postId)
        {
            var report = await _analyticsService.GenerateEngagementReportAsync(postId);
            return Ok(report);
        }



        //comments
        [HttpPost("{postId}/comments")]
        public async Task<ActionResult<string>> AddComment(string postId, CreateCommentRequest request)
        {
            try
            {
                var comment = new Comment
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = request.Content,
                    Author = new User
                    {
                        UserId = GetCurrentUserId(),
                        Name = GetCurrentUserName()
                    },
                    MediaItems = request.MediaItems?.Select(m => new MediaItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContentType = m.ContentType,
                        Url = m.Url,
                        Size = m.Size,
                        Type = m.Type
                    }).ToList() ?? new List<MediaItem>(),
                    UserTags = request.UserTags?.Select(userId => new UserTag
                    {
                        UserId = userId
                    }).ToList() ?? new List<UserTag>(),
                    ParentComment = request.ParentComment,
                    CreatedAt = DateTime.UtcNow,
                    Status = CommentStatus.Active
                };

                await _postRepository.AddCommentAsync(postId, comment);
                await _analyticsService.UpdateMetricsAsync(postId);

                return Ok(new { Id = comment.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        //reactions
        [HttpPost("{postId}/reactions")]
        public async Task<ActionResult> AddReaction(string postId, CreateReactionRequest request)
        {
            try
            {
                var reaction = new Reaction
                {
                    ReactionId = Guid.NewGuid().ToString(),
                    UserId = GetCurrentUserId().ToString(),
                    Type = request.Type,
                    CreatedAt = DateTime.UtcNow
                };

                await _postRepository.AddReactionAsync(postId, reaction);
                await _analyticsService.UpdateMetricsAsync(postId);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        [HttpDelete("{postId}/reactions")]
        public async Task<ActionResult> RemoveReaction(string postId)
        {
            try
            {
                await _postRepository.RemoveReactionAsync(postId, GetCurrentUserId().ToString());
                await _analyticsService.UpdateMetricsAsync(postId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        //shares
        [HttpPost("{postId}/shares")]
        public async Task<ActionResult<string>> SharePost(string postId, CreateShareRequest request)
        {
            try
            {
                var share = new Share
                {
                    ShareId = Guid.NewGuid().ToString(),
                    UserId = GetCurrentUserId().ToString(),
                    Message = request.Message,
                    CreatedAt = DateTime.UtcNow,
                    VisibilitySettings = request.VisibilitySettings ?? new VisibilitySettings
                    {
                        Type = VisibilityType.Public
                    }
                };

                await _postRepository.AddShareAsync(postId, share);
                await _analyticsService.UpdateMetricsAsync(postId);

                return Ok(new { Id = share.ShareId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("feed")]
        public async Task<ActionResult<FeedResult>> GetFeed([FromQuery] int page = 1)
        {
            var userId = GetCurrentUserId(); // Get from auth context
            var result = await _postRepository.GetUserFeedAsync(userId, page);
            return Ok(result);
        }
        [HttpGet("videos")]
        public async Task<ActionResult<IEnumerable<Post>>> GetVideoFeed([FromQuery] int page = 1)
        {
            try
            {
                var userId = GetCurrentUserId();
                var posts = await _postRepository.GetVideoPostsAsync(userId, page);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving video posts");
                return BadRequest(new { Error = ex.Message });
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

        private string GetCurrentUserType()
        {
            var userType = HttpContext.Request.Headers["X-UserType"].FirstOrDefault();
            if (string.IsNullOrEmpty(userType))
            {
                throw new UnauthorizedAccessException("User type not found in request headers");
            }
            return userType;
        }

        private string GetCurrentUserName()
        {
            // In a real application, you might want to get this from a user service or cache
            return "User " + GetCurrentUserId().ToString();
        }

        private string GetCurrentUserAvatar()
        {
            return User.FindFirst("avatar")?.Value ?? "";
        }
    }

}
