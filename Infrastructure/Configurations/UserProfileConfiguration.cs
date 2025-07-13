using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGameServer.Domain.Entities;

namespace QuizGameServer.Infrastructure.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
            builder.Property(x => x.AvatarId).IsRequired().HasMaxLength(50);
        }
    }
}
