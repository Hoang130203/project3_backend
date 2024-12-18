using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.CQRS.Commands.Groups.CreateGroup
{
    public record CreateGroupCommand : ICommand<CreateGroupResult>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public GroupVisibility Visibility { get; init; }
        public Guid CreatorId { get; init; }
    }

    public record CreateGroupResult(Guid GroupId);
}
