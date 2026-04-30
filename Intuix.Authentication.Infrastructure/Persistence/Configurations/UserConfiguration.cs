using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intuix.Authentication.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("auth_users");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.Username).HasColumnName("username");
        builder.Property(x => x.Email).HasColumnName("email");
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash");
        builder.Property(x => x.IsActive).HasColumnName("is_active");
        builder.Property(x => x.IsLocked).HasColumnName("is_locked");
        builder.Property(x => x.FailedAttempts).HasColumnName("failed_attempts");
        builder.Property(x => x.LastLogin).HasColumnName("last_login");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.Username })
            .IsUnique();

        //builder.HasOne<Tenant>()
        //    .WithMany()
        //    .HasForeignKey(x => x.TenantId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //builder.HasMany(x => x.RefreshTokens)
        //    .WithOne(x => x.User)
        //    .HasForeignKey(x => x.UserId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}