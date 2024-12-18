using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.OwnerId).IsRequired();
            builder.HasOne(x => x.Owner).WithMany().HasForeignKey(x => x.OwnerId);
            //builder.HasMany(x => x.Memberships).WithOne().HasForeignKey(x => x.GroupId);
            builder.Property(x => x.Visibility).IsRequired();
            builder.HasMany(x => x.Memberships)
                .WithOne(m => m.Group)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
