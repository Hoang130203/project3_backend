using AuthService.Domain.Entities.Permissions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class RoleGroupPermissionMappingConfiguration: IEntityTypeConfiguration<RoleGroupPermissionMapping>
    {

        public void Configure(EntityTypeBuilder<RoleGroupPermissionMapping> builder)
        {
            // Composite Primary Key
            builder.HasKey(x => new { x.RoleGroupId, x.PermissionId });

            // Required Properties
            builder.Property(x => x.RoleGroupId)
                .IsRequired();

            builder.Property(x => x.PermissionId)
                .IsRequired();

            // Explicitly define relationships with clear foreign key specifications
            builder.HasOne(rgpm => rgpm.RoleGroup)
                .WithMany(rg => rg.RoleGroupPermissions)
                .HasForeignKey(rgpm => rgpm.RoleGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rgpm => rgpm.Permission)
                .WithMany()
                .HasForeignKey(rgpm => rgpm.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
