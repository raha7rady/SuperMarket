
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SuperMarket.Infrastructure.Identity;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasIndex(u => u.DomainUserId).IsUnique();
        builder.Property(u => u.DomainUserId).IsRequired();

        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.UserName).HasMaxLength(256);
    }
}
