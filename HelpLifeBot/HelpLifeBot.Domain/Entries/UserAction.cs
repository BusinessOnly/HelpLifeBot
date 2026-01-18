using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpLifeBot.Domain
{
    public sealed class UserAction
    {
        public long UserActionId { get; set; }
        public Guid UserActionKey { get; set; }
        public long UserId { get; set; }
        public string ActionCode { get; set; } = null!;
        public string? ActionDetails { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public sealed class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
    {
        public void Configure(EntityTypeBuilder<UserAction> builder)
        {
            builder.ToTable("UserActions");
            builder.HasKey(b => b.UserActionId);
            builder.Property(b => b.UserActionId)
                   .ValueGeneratedOnAdd()
                   .UseIdentityColumn();
            builder.Property(b => b.UserActionKey)
                   .IsRequired()
                   .HasDefaultValueSql("gen_random_uuid()");
            builder.Property(b => b.UserId)
                   .IsRequired();
            builder.Property(b => b.ActionCode)
                   .IsRequired()
                   .HasMaxLength(64);
            builder.Property(b => b.ActionDetails);
            builder.Property(b => b.CreatedOn)
                   .IsRequired()
                   .HasDefaultValueSql("timezone('utc', now())");

            builder.HasIndex(x => x.UserActionKey)
                   .IsUnique();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => new { x.UserId, x.CreatedOn });

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .HasPrincipalKey(u => u.UserId);
        }
    }
}
