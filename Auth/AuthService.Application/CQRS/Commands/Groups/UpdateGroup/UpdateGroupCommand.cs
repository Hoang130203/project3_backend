using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.CQRS.Commands.Groups.UpdateGroup
{
    public record UpdateGroupCommand : ICommand<UpdateGroupResult>
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public GroupVisibility Visibility { get; init; }
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
    }

    public record UpdateGroupResult(Guid GroupId);
}
