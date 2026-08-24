
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration
    : IEntityTypeConfiguration<CartItem>
{
    private const string TableName = "CartItems";

    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
               .ValueGeneratedNever();

        builder.Property(ci => ci.CartId)
               .IsRequired();

        builder.Property(ci => ci.ProductId)
               .IsRequired();

        builder.Property(ci => ci.CreatedDate)
               .IsRequired();

        builder.Property(ci => ci.CreatedBy);

        builder.Property(ci => ci.LastModifiedDate);

        builder.Property(ci => ci.LastModifiedBy);

        builder.Property(ci => ci.IsDeleted)
               .IsRequired();

        builder.Property(ci => ci.DeletedDate);

        builder.Property(ci => ci.DeletedBy);

        builder.Property<byte[]>("RowVersion")
               .IsRowVersion();

        // -------------------------
        // ProductTitle
        // -------------------------

        builder.OwnsOne(ci => ci.Title, title =>
        {
            title.Property(x => x.Value)
                 .HasColumnName("Title")
                 .HasMaxLength(200)
                 .IsRequired();
        });

        // -------------------------
        // Toman
        // -------------------------

        builder.OwnsOne(ci => ci.Price, price =>
        {
            price.Property(x => x.Amount)
                 .HasColumnName("Price")
                 .HasColumnType("decimal(18,0)")
                 .IsRequired();
        });

        // -------------------------
        // Quantity
        // -------------------------

        builder.OwnsOne(ci => ci.Quantity, quantity =>
        {
            quantity.Property(x => x.Value)
                    .HasColumnName("Quantity")
                    .IsRequired();
        });

        // -------------------------
        // Cart
        // -------------------------

        //builder.HasOne(ci => ci.Cart)
        //       .WithMany(c => c.Items)
        //       .HasForeignKey(ci => ci.CartId)
        //       .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Product
        // -------------------------

        builder.HasOne(ci => ci.Product)
               .WithMany()
               .HasForeignKey(ci => ci.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        // -------------------------
        // Indexes
        // -------------------------

        builder.HasIndex(ci => ci.ProductId);

        builder.HasIndex(ci => ci.IsDeleted);

        builder.HasIndex(ci => new
        {
            ci.CartId,
            ci.ProductId,
            ci.IsDeleted
        })
        .IsUnique()
        .HasDatabaseName("IX_CartItems_Cart_Product");
    }
}