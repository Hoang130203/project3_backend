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
    public class GroupMembershipConfiguration : IEntityTypeConfiguration<GroupMembership>
    {
        public void Configure(EntityTypeBuilder<GroupMembership> builder)
        {
            builder.HasKey(x => x.Id);

            // Required Properties
            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.GroupId)
                .IsRequired();

            builder.Property(x => x.Role)
                .IsRequired();

            builder.Property(x => x.JoinedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Group)
                .WithMany(g => g.Memberships)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint to prevent duplicate memberships
            builder.HasIndex(x => new { x.UserId, x.GroupId })
                .IsUnique();
        }
    }
}
