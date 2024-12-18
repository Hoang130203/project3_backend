using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Entities.Groups;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class RoleGroupConfiguration : IEntityTypeConfiguration<RoleGroup>
    {
        public void Configure(EntityTypeBuilder<RoleGroup> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Role).IsRequired();
            builder.Property(x => x.IsAllowed).IsRequired();
            //builder.HasMany(x => x.Permissions).WithMany().UsingEntity<RoleGroupPermissionMapping>(
            //    j => j.HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId),
            //    j => j.HasOne(x => x.RoleGroup).WithMany().HasForeignKey(x => x.RoleGroupId)
            //);
            //builder.Property(x => x.GroupId).IsRequired(false);
            // Optional Group Relationships
            builder.HasOne(rg => rg.Group)
             .WithMany()
             .HasForeignKey(rg => rg.AssociatedGroupId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Permissions
            builder.HasMany(rg => rg.RoleGroupPermissions)
                .WithOne(rgpm => rgpm.RoleGroup)
                .HasForeignKey(rgpm => rgpm.RoleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }

}
