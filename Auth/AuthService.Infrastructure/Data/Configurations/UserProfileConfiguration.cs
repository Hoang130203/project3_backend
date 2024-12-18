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
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.FullName);
            builder.Property(x => x.DateOfBirth);
            builder.Property(x => x.ProfilePictureUrl);
            builder.Property(x => x.IsMale);
            builder.Property(x => x.Bio);
            builder.Property(x => x.Location);
            builder.Property(x => x.Website);
            builder.HasOne(x => x.User).WithOne(x => x.Profile).HasForeignKey<UserProfile>(x => x.UserId);
            builder.Property(x => x.Email);
            builder.Property(x => x.Phone);
            builder.Property(x => x.maritalStatus);
        }
    }
}
