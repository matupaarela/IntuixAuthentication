using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intuix.Authentication.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("auth_organizations");

        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.IsActive).HasColumnName("is_active");

        // 🔹 PK
        builder.HasKey(x => x.Id);

        // 🔹 Propiedades
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TenantId)
            .IsRequired();

        // 🔹 Índice (importante en multi-tenant)
        builder.HasIndex(x => new { x.TenantId, x.Name });

        // 🔹 Relación con Tenant (🔥 SIN CASCADE)
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict); // 🔥 CLAVE

        // 🔹 Relación con Companies
        builder.HasMany(x => x.Companies)
            .WithOne(x => x.Organization)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict); // 🔥 CLAVE
    }
}
