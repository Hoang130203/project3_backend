using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialAppObjects
{
    public record SubInfo
    {
        public Guid Id { get; init; } = Guid.Empty;
        public string Name { get; init; } = string.Empty;
        public string AvatarUrl { get; init; } = string.Empty;
    }
}
