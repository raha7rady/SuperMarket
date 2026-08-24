using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private const string TableName = "Users";

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(u => u.Id);

            // =========================
            // Name (Owned)
            // =========================
            builder.OwnsOne(u => u.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("FirstName")
                    .HasMaxLength(50)
                    .IsRequired();

                name.Property(n => n.LastName)
                    .HasColumnName("LastName")
                    .HasMaxLength(50)
                    .IsRequired();
            });
            builder.Navigation(u => u.Name).IsRequired();

            // =========================
            // Email (Owned)
            // =========================
            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                     .HasColumnName("UserEmail")
                     .HasColumnType("nvarchar(200)")
                     .IsRequired();

                // 🔹 index معتبر روی owned property
                email.HasIndex(e => e.Value)
                     .IsUnique()
                     .HasDatabaseName("IX_Users_UserEmail");
            });
            builder.Navigation(u => u.Email).IsRequired();

            // =========================
            // PasswordHash (Owned)
            // =========================
            builder.OwnsOne(u => u.PasswordHash, hash =>
            {
                hash.Property(p => p.Value)
                    .HasColumnName("PasswordHash")
                    .HasMaxLength(500)
                    .IsRequired();
            });
            builder.Navigation(u => u.PasswordHash).IsRequired();

            // =========================
            // Role
            // =========================
            builder.Property(u => u.Role)
                   .IsRequired()
                   .HasConversion<string>();

            // =========================
            // Auditable Fields
            // =========================
            builder.Property(u => u.CreatedDate).IsRequired();
            builder.Property(u => u.CreatedBy).IsRequired();
            builder.Property(u => u.LastModifiedDate);
            builder.Property(u => u.LastModifiedBy);
            builder.Property(u => u.IsDeleted).IsRequired();
            builder.Property(u => u.DeletedDate);
            builder.Property(u => u.DeletedBy);

            // =========================
            // Relationships
            // =========================
            builder.HasMany(u => u.Orders)
                   .WithOne(o => o.User)
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Carts)
                   .WithOne(c => c.User)
                   .HasForeignKey(c => c.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Metadata
                   .FindNavigation(nameof(User.Orders))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.Metadata
                   .FindNavigation(nameof(User.Carts))!
                   .SetPropertyAccessMode(PropertyAccessMode.Field);

            // =========================
            // Indexes دیگر
            // =========================
            builder.HasIndex(u => u.IsDeleted);
            builder.HasIndex(u => u.Role);
        }
    }
}
