using Auth;
using AuthService.Application.Interfaces.Repositories;
using Grpc.Core;

namespace AuthService
{
    public class ConnectionGrpcService : ConnectionServiceGrpc.ConnectionServiceGrpcBase
    {
        private readonly IUserRepository _userRepository;
        public ConnectionGrpcService(
            IUserRepository userRepository
            ) { 
            _userRepository = userRepository;
        }

        public override async Task<UserConnectionsResponse> GetUserConnections(UserConnectionsRequest userConnectionsRequest,
       ServerCallContext context)
        {
            var friends = await _userRepository.GetFriendsIdAsync(Guid.Parse(userConnectionsRequest.UserId));
            var groups = await _userRepository.GetGroupsIdAsync(Guid.Parse(userConnectionsRequest.UserId));

            return new UserConnectionsResponse
            {
                FriendIds = { friends },
                GroupIds = { groups }
            };
        }
    }
}
