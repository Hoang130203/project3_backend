using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get;  }
        DbSet<UserProfile> UserProfiles { get;  }
        DbSet<UserRelationship> UserRelationships { get;  }
        DbSet<Group> Groups { get;  }
        DbSet<GroupInvitation> GroupInvitations { get;  }
        DbSet<GroupMembership> GroupMembership { get; }
        DbSet<Permission> Permissions { get;  }
        DbSet<RoleGroup> RoleGroups { get; }
        DbSet<RoleGroupPermissionMapping> RoleGroupPermissionMappings { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
