
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations;

public sealed class CartConfiguration
    : IEntityTypeConfiguration<Cart>
{
    private const string TableName = "Carts";

    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .ValueGeneratedNever();

        builder.Property(c => c.UserId)
               .IsRequired();

        builder.Property(c => c.CreatedDate)
               .IsRequired();

        builder.Property(c => c.CreatedBy);

        builder.Property(c => c.LastModifiedDate);

        builder.Property(c => c.LastModifiedBy);

        builder.Property(c => c.IsDeleted)
               .IsRequired();

        builder.Property(c => c.DeletedDate);

        builder.Property(c => c.DeletedBy);

        // -------------------------
        // User
        // -------------------------

        builder.HasOne(c => c.User)
               .WithMany(u => u.Carts)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Cart Items
        // -------------------------

        builder.HasMany(c => c.Items)
               .WithOne(ci => ci.Cart)
               .HasForeignKey(ci => ci.CartId)
               .OnDelete(DeleteBehavior.Restrict);

        // مهم
        builder.Metadata
               .FindNavigation(nameof(Cart.Items))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        // اتصال مستقیم به فیلد private
        builder.Navigation(c => c.Items)
               .HasField("_items");

        builder.Navigation(c => c.Items)
       .UsePropertyAccessMode(PropertyAccessMode.Field);

        // -------------------------
        // Index
        // -------------------------

        builder.HasIndex(c => new
        {
            c.UserId,
            c.IsDeleted
        })
        .HasDatabaseName("IX_Carts_User_Active");
    }
}