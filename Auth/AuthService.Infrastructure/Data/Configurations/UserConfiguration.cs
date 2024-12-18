using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username).IsRequired();
            builder.HasIndex(x => x.Username)
               .IsUnique();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.UserType).IsRequired();
            builder.HasOne(x => x.Profile).WithOne().HasForeignKey<User>(x => x.Id);
            builder.HasMany(x => x.Relationships)
                .WithOne(r => r.SourceUser)
                .HasForeignKey(r => r.SourceUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.GroupMemberships)
                .WithOne(gm => gm.User)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
