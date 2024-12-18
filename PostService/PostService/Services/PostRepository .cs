using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using PostService.Infrastructure;
using PostService.Interfaces;
using PostService.Models;
using PostService.Protos;
using SocialAppObjects.Enums;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace PostService.Services
{
    public class FeedResult
    {
        public List<Post> Posts { get; set; }
        public int Page { get; set; }
        public bool HasMore { get; set; }
    }

    public class UserConnections
    {
        public List<Guid> FriendIds { get; set; }
        public List<Guid> GroupIds { get; set; }
    }
    public class PostRepository : IPostRepository
    {
        private readonly IMongoCollection<Post> _posts;
        private readonly IMediaService _mediaService;
        private readonly IPostModerationService _moderationService;
        private readonly IConnectionGrpcClient _connectionGrpcClient;
        private readonly IProfileGrpcClient _profileGrpcClient;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(
            MongoDbClient mongoClient,
            IMediaService mediaService,
            IPostModerationService moderationService,
            IConnectionGrpcClient connectionGrpcClient,
            IProfileGrpcClient profileGrpcClient,
            ILogger<PostRepository> logger
            )
        {
            _posts = mongoClient.Posts;
            _mediaService = mediaService;
            _moderationService = moderationService;
            _connectionGrpcClient = connectionGrpcClient;
            _profileGrpcClient = profileGrpcClient;
            _logger = logger;
        }

        private async Task EnrichWithProfileInfo(Post post)
        {
            try
            {
                // Fetch author info
                var authorInfo = await _profileGrpcClient.GetSubInfoAsync(post.Author.UserId.ToString());
                post.Author.Name = authorInfo.Name;
                post.Author.Avatar = authorInfo.AvatarUrl;

                // Fetch group info if present
                if (post.Group != null)
                {
                    var groupInfo = await _profileGrpcClient.GetSubInfoAsync(post.Group.Id.ToString());
                    post.Group.Name = groupInfo.Name;
                    post.Group.Avatar = groupInfo.AvatarUrl;
                }

                // Fetch info for all comments authors
                if (post.Comments?.Any() == true)
                {
                    var commentAuthorIds = post.Comments
                        .Select(c => c.Author.UserId.ToString())
                        .Distinct()
                        .ToList();

                    var commentAuthorsInfo = await _profileGrpcClient.GetMultipleSubInfoAsync(commentAuthorIds);
                    var authorInfoMap = commentAuthorsInfo.SubInfos
                        .ToDictionary(info => info.Id, info => (info.Name, info.AvatarUrl));

                    foreach (var comment in post.Comments)
                    {
                        if (authorInfoMap.TryGetValue(comment.Author.UserId.ToString(), out var authorInfoo))
                        {
                            comment.Author.Name = authorInfoo.Name;
                            comment.Author.Avatar = authorInfoo.AvatarUrl;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enriching post {PostId} with profile information", post.Id);
            }
        }

        private async Task EnrichPostsWithProfileInfo(IEnumerable<Post> posts)
        {
            foreach (var post in posts)
            {
                await EnrichWithProfileInfo(post);
            }
        }
        public async Task<IEnumerable<Post>> GetUserFeedAsync(Guid userId, int skip, int take)
        {
            var filter = Builders<Post>.Filter.And(
                Builders<Post>.Filter.Eq("Status", PostStatus.Active),
                Builders<Post>.Filter.Or(
                    Builders<Post>.Filter.Eq("Visibility.Type", VisibilityType.Public),
                    Builders<Post>.Filter.Eq("Author.UserId", userId),
                    Builders<Post>.Filter.AnyEq("Visibility.AllowedUserIds", userId)
                )
            );

            return await _posts.Find(filter)
                             .Sort(Builders<Post>.Sort.Descending(p => p.CreatedAt))
                             .Skip(skip)
                             .Limit(take)
                             .ToListAsync();
        }

        public async Task<IEnumerable<Post>> SearchAsync(PostSearchParams searchParams)
        {
            var builder = Builders<Post>.Filter;
            var filters = new List<FilterDefinition<Post>>();

            // Authors
            if (searchParams.AuthorIds?.Any() == true)
            {
                filters.Add(builder.In("Author.UserId", searchParams.AuthorIds));
            }

            // Groups  
            if (searchParams.GroupIds?.Any() == true)
            {
                filters.Add(builder.In("Group.Id", searchParams.GroupIds));
            }

            // Location-based search
            if (searchParams.NearLocation != null && searchParams.RadiusKm.HasValue)
            {
                var point = GeoJson.Point(new GeoJson2DGeographicCoordinates(
                    searchParams.NearLocation.Longitude,
                    searchParams.NearLocation.Latitude
                ));

                filters.Add(builder.Near("Location.Coordinates", point, maxDistance: searchParams.RadiusKm.Value * 1000));
            }

            var filter = filters.Any()
                ? builder.And(filters)
                : builder.Empty;

            var sort = Builders<Post>.Sort.Descending("CreatedAt");

            return await _posts.Find(filter)
                              .Sort(sort)
                              .Skip(searchParams.Skip)
                              .Limit(searchParams.Take)
                              .ToListAsync();
        }


        private List<string> ExtractHashtags(string content)
        {
            var hashtags = new HashSet<string>();
            var regex = new Regex(@"#(\w+)");
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                hashtags.Add(match.Groups[1].Value.ToLower());
            }

            return hashtags.ToList();
        }

        public async Task<Post> GetByIdAsync(string id)
        {
            var post = await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (post != null)
            {
                await EnrichWithProfileInfo(post);
            }
            return post;
        }

        public async Task<IEnumerable<Post>> GetGroupPostsAsync(Guid groupId, int skip, int take)
        {
            var filter = Builders<Post>.Filter.And(
               Builders<Post>.Filter.Eq("Status", PostStatus.Active),
               Builders<Post>.Filter.Eq("Group.Id", groupId)
           );

            var sort = Builders<Post>.Sort.Descending("CreatedAt");

            var posts = await _posts.Find(filter)
                             .Sort(sort)
                             .Skip(skip)
                             .Limit(take)
                             .ToListAsync();

            await EnrichPostsWithProfileInfo(posts);
            return posts;
        }

        public async Task<string> CreateAsync(Post post)
        {
            try
            {
                // Validate post content
                var isValid = await ValidatePostContentAsync(post);
                if (!isValid)
                {
                    throw new InvalidOperationException("Post content validation failed");
                }

                // Process media items if any
                if (post.MediaItems?.Any() == true)
                {
                    foreach (var mediaItem in post.MediaItems)
                    {
                        var result = await _mediaService.ProcessMediaAsync(mediaItem);
                        if (!result.Success)
                        {
                            throw new InvalidOperationException($"Media processing failed: {string.Join(", ", result.Errors)}");
                        }
                        mediaItem.Url = result.ProcessedUrl;
                        mediaItem.ThumbnailUrl = result.ThumbnailUrl;
                    }
                }

                // Extract hashtags and process location
                post.HashTags = ExtractHashtags(post.Content);
                post.Location?.SetCoordinates();

                // Set timestamps
                post.CreatedAt = DateTime.UtcNow;
                post.UpdatedAt = DateTime.UtcNow;

                // Initialize metrics
                post.Metrics = new PostMetrics
                {
                    LastEngagementAt = DateTime.UtcNow
                };

                await _posts.InsertOneAsync(post);
                return post.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating post: {ex.Message}", ex);
            }
        }


        public async Task UpdateAsync(string id, Post post)
        {
            var filter = Builders<Post>.Filter.Eq("_id", id);
            var update = Builders<Post>.Update
                .Set("Title", post.Title)
                .Set("Content", post.Content)
                .Set("HashTags", ExtractHashtags(post.Content))
                .Set("UpdatedAt", DateTime.UtcNow);

            if (post.Location != null)
            {
                post.Location.SetCoordinates();
                update = update.Set("Location", post.Location);
            }

            await _posts.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<Post>.Filter.Eq("_id", id);
            var update = Builders<Post>.Update
                .Set("Status", PostStatus.Deleted)
                .Set("UpdatedAt", DateTime.UtcNow);

            await _posts.UpdateOneAsync(filter, update);
        }



        public async Task<bool> ValidatePostContentAsync(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Content) && !post.MediaItems?.Any() == true)
            {
                return false;
            }

            if (post.Content?.Length > 5000) // Maximum content length
            {
                return false;
            }

            // Check for media validation
            //if (post.MediaItems?.Any() == true)
            //{
            //    foreach (var mediaItem in post.MediaItems)
            //    {
            //        if (string.IsNullOrEmpty(mediaItem.Url) || mediaItem.Size > 10_000_000) // 10MB limit
            //        {
            //            return false;
            //        }
            //    }
            //}
            var result= await _moderationService.ModerateContentAsync(post);
            if (!result.IsApproved)
            {
                return false;
            }
            // More validation rules can be added here
            return true;
        }

        public async Task UpdateMediaAsync(string postId, UpdateDefinition<Post> update)
        {
            var filter = Builders<Post>.Filter.Eq("_id", postId);
            await _posts.UpdateOneAsync(filter, update);
        }


        public async Task AddCommentAsync(string postId, Comment comment)
        {
            if (comment.ParentComment != null)
            {
                // Tìm ParentComment trong danh sách Comments của Post
                var filter = Builders<Post>.Filter.And(
                    Builders<Post>.Filter.Eq(p => p.Id, postId),
                    Builders<Post>.Filter.ElemMatch(p => p.Comments, c => c.Id == comment.ParentComment)
                );

                var post = await _posts.Find(filter).FirstOrDefaultAsync();

                if (post == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy ParentComment với Id: {comment.ParentComment} trong bài post với Id: {postId}");
                }
            }

            var update = Builders<Post>.Update
                .Push(p => p.Comments, comment)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p => p.Id, postId),
                update
            );

            if (result.ModifiedCount == 0)
            {
                // Xử lý trường hợp không tìm thấy bài post để update
                throw new InvalidOperationException($"Không tìm thấy bài post với Id: {postId}");
            }
        }

        public async Task AddReactionAsync(string postId, Reaction reaction)
        {
            // Remove existing reaction from same user if exists
            await RemoveReactionAsync(postId, reaction.UserId);

            var update = Builders<Post>.Update
                .Push(p=>p.Reactions, reaction)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            await _posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p => p.Id, postId),
                update
            );
        }

        public async Task RemoveReactionAsync(string postId, string userId)
        {
            var update = Builders<Post>.Update
                .PullFilter(p=>p.Reactions, Builders<Reaction>.Filter.Eq(r=>r.UserId, userId));

            await _posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p=>p.Id, postId),
                update
            );
        }

        public async Task AddShareAsync(string postId, Share share)
        {
            var update = Builders<Post>.Update
                .Push(p=>p.Shares, share);

            await _posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq(p=>p.Id, postId),
                update
            );
        }

        public async Task<IEnumerable<FeedResult>> GetUserFeedAsync(Guid Id, int page)
        {
            var connections = await GetUserConnectionsAsync(Id);
            var friendIds = connections.FriendIds;
            var groupIds = connections.GroupIds;

            var skip = (page - 1) * 20;
            var take = 20;
            var builder = Builders<Post>.Filter;
            var filters = new List<FilterDefinition<Post>>();

            //filters.Add(builder.Eq(p => p.Status, PostStatus.Active));

            if (friendIds.Any())
            {
                var friendFilter = builder.And(
                    builder.In(p => p.Author.UserId, friendIds)
                    //builder.Or(
                    //    builder.Eq(p => p.Visibility.Type, VisibilityType.Public),
                    //    builder.Eq(p => p.Visibility.Type, VisibilityType.Friends)
                    //)
                );
                filters.Add(friendFilter);
            }

            if (groupIds.Any())
            {
                var groupFilter = builder.And(
                    builder.In("Group.Id", groupIds),
                    builder.Ne(p => p.Visibility.Type, VisibilityType.Private)
                );
                filters.Add(groupFilter);
            }

            var combinedFilter = builder.Or(filters);

            var posts = await _posts.Find(combinedFilter)
                .SortByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            await EnrichPostsWithProfileInfo(posts);

            return new List<FeedResult>
            {
                new FeedResult
                {
                    Posts = posts,
                    Page = page,
                    HasMore = posts.Count == take
                }
            };
        }

        public async Task<IEnumerable<Post>> GetVideoPostsAsync(Guid userId, int page)
        {
            var skip = (page - 1) * 20;
            var take = 20;

            var connections = await GetUserConnectionsAsync(userId);
            var friendIds = connections.FriendIds;
            var groupIds = connections.GroupIds;

            var builder = Builders<Post>.Filter;
            var filters = new List<FilterDefinition<Post>>();

            // Base filters for active posts
            //filters.Add(builder.Eq(p => p.Status, PostStatus.Active));

            // Filter for posts containing video media
            filters.Add(builder.ElemMatch(p => p.MediaItems, m => m.ContentType.StartsWith("video")));

            // Friend posts filter - public or friends-only posts
            if (friendIds.Any())
            {
                var friendFilter = builder.And(
                    builder.In(p => p.Author.UserId, friendIds)
                    //, builder.Or(
                    //    builder.Eq(p => p.Visibility.Type, VisibilityType.Public),
                    //    builder.Eq(p => p.Visibility.Type, VisibilityType.Friends)
                    //)
                );
                filters.Add(friendFilter);
            }

            // Group posts filter
            if (groupIds.Any())
            {
                var groupFilter = builder.And(
                    builder.Or(
                        builder.In(p => p.Group.Id, groupIds),
                        builder.Where(p => p.Group == null)
                    )
                    
                    //builder.Ne(p => p.Visibility.Type, VisibilityType.Private)
                );
                filters.Add(groupFilter);
            }

            // Combine all filters
            var combinedFilter = builder.And(filters);

            // Project to include only the first video
            var projection = Builders<Post>.Projection
                .Include(p => p.Id)
                .Include(p => p.Title)
                .Include(p => p.MediaItems)
                .Include(p => p.Content)
                .Include(p => p.Author)
                .Include(p => p.Group)
                .Include(p => p.CreatedAt)
                .Include(p => p.UpdatedAt)
                .Include(p => p.Metrics);

            var posts = await _posts.Find(combinedFilter)
                .Project<Post>(projection)
                .SortByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Limit(take)
                .ToListAsync();

            // Process posts to keep only first video
            foreach (var post in posts)
            {
                var videoMedia = post.MediaItems?
                    .FirstOrDefault(m => m.ContentType.StartsWith("video"));

                post.MediaItems = videoMedia != null
                    ? new List<MediaItem> { videoMedia }
                    : new List<MediaItem>();
            }
            await EnrichPostsWithProfileInfo(posts);
            return posts;
        }
        private async Task<UserConnections> GetUserConnectionsAsync(Guid userId)
        {
            //var request = new UserConnectionsRequest
            //{
            //    UserId = userId.ToString()
            //};

            var response = await _connectionGrpcClient.GetUserConnectionsAsync(userId.ToString());

            return new UserConnections
            {
                FriendIds = response.FriendIds.Select(Guid.Parse).ToList(),
                GroupIds = response.GroupIds.Select(Guid.Parse).ToList()
            };
        }
    }
}
