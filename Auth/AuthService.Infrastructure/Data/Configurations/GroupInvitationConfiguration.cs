using AuthService.Domain.Entities.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Configurations
{
    public class GroupInvitationConfiguration : IEntityTypeConfiguration<GroupInvitation>
    {
        public void Configure(EntityTypeBuilder<GroupInvitation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.GroupId).IsRequired();
            builder.Property(x => x.InvitedUserId).IsRequired();
            builder.Property(x => x.InviterId).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.InvitedAt).IsRequired();
            builder.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId);
            builder.HasOne(x => x.InvitedUser).WithMany().HasForeignKey(x => x.InvitedUserId);
            builder.HasOne(x => x.Inviter).WithMany().HasForeignKey(x => x.InviterId);


            // Cấu hình mối quan hệ giữa GroupInvitation và User (InvitedUser)
            builder
                .HasOne(gi => gi.InvitedUser)
                .WithMany()  // Mỗi người dùng có thể nhận nhiều lời mời
                .HasForeignKey(gi => gi.InvitedUserId)
                .OnDelete(DeleteBehavior.Restrict);  // Khi người được mời bị xóa, không xóa lời mời

            // Cấu hình mối quan hệ giữa GroupInvitation và Group
            builder
                .HasOne(gi => gi.Group)
                .WithMany()  // Mỗi nhóm có thể có nhiều lời mời
                .HasForeignKey(gi => gi.GroupId)
                .OnDelete(DeleteBehavior.Restrict);  // Khi nhóm bị xóa, không xóa lời mời

            builder
                .HasOne(gi => gi.Inviter)
                .WithMany()  // Mỗi người dùng có thể gửi nhiều lời mời
                .HasForeignKey(gi => gi.InviterId)
                .OnDelete(DeleteBehavior.Restrict);  // Khi người mời bị xóa, không xóa lời mời


        }
    }
}
