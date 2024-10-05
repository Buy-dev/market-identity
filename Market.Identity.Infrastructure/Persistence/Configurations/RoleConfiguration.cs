using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Identity.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(30)
            .IsRequired();
        
        builder.Property(p => p.Title)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(200)
            .IsRequired();
    }
}