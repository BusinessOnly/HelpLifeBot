using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpLifeBot.Domain
{
    public sealed class User
    {
        public long UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public bool IsBot { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(b => b.UserId);
            builder.Property(b => b.FirstName)
                   .IsRequired()
                   .HasMaxLength(256);
            builder.Property(b => b.LastName)
                   .HasMaxLength(256);
            builder.Property(b => b.UserName)
                   .HasMaxLength(64);
            builder.Property(b => b.IsBot);
            builder.Property(b => b.IsPremium);
            builder.Property(b => b.CreatedOn)
                   .IsRequired()
                   .HasDefaultValueSql("timezone('utc', now())");
            builder.Property(b => b.UpdatedOn);

            builder.HasIndex(x => x.UserName)
                .IsUnique();
        }
    }
}
