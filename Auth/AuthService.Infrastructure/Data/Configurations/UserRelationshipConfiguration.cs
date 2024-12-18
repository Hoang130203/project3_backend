using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class UserRelationshipConfiguration : IEntityTypeConfiguration<UserRelationship>
    {
        public void Configure(EntityTypeBuilder<UserRelationship> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Required Properties
            builder.Property(x => x.SourceUserId)
                .IsRequired();

            builder.Property(x => x.TargetUserId)
                .IsRequired();

            builder.Property(x => x.Status)
                ;

            builder.Property(x => x.RequestedAt)
                ;

            // Relationships


            builder.HasOne(x => x.TargetUser)
                .WithMany()
                .HasForeignKey(x => x.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Constraint to prevent self-relationships
            //builder.ToTable("CHK_Different_Users",
            //    "[SourceUserId] != [TargetUserId]");
        }
    }
}
