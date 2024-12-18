using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public record UpdateGroupEvent : IntegrationEvent
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public GroupVisibility Visibility { get; set; }
    }
}
