using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Market.Identity.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Username)
            .IsUnique();
        
        builder.Property(u => u.Username)
            .HasMaxLength(40)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(254)
            .IsRequired();
        
        builder.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CallSign)
            .HasMaxLength(40);
    }
}