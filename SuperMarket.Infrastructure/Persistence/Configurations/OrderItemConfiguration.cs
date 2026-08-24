using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        private const string TableName = "OrderItems";

        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.OrderId)
                   .IsRequired();

            builder.Property(oi => oi.ProductId)
                   .IsRequired();

            // ========================
            // Audit Fields
            // ========================

            builder.Property(oi => oi.CreatedDate).IsRequired();
            builder.Property(oi => oi.CreatedBy).IsRequired();
            builder.Property(oi => oi.LastModifiedDate);
            builder.Property(oi => oi.LastModifiedBy);
            builder.Property(oi => oi.IsDeleted).IsRequired();
            builder.Property(oi => oi.DeletedDate);
            builder.Property(oi => oi.DeletedBy);

            builder.Property<byte[]>("RowVersion")
                   .IsRowVersion();

            // ========================
            // Value Objects (Owned)
            // ========================

            builder.OwnsOne(oi => oi.Title, t =>
            {
                t.Property(p => p.Value)
                 .HasColumnName("Title")
                 .HasMaxLength(200)
                 .IsRequired();
            });

            builder.OwnsOne(oi => oi.Price, p =>
            {
                p.Property(x => x.Amount)
                 .HasColumnName("Price")
                 .HasPrecision(18, 0)
                 .IsRequired();
            });

            builder.OwnsOne(oi => oi.Quantity, q =>
            {
                q.Property(x => x.Value)
                 .HasColumnName("Quantity")
                 .IsRequired();
            });

            // ========================
            // Relationships
            // ========================

            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.Items)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.Product)
                   .WithMany()
                   .HasForeignKey(oi => oi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ========================
            // Indexes
            // ========================

            builder.HasIndex(oi => new { oi.OrderId, oi.ProductId })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("IX_OrderItems_Order_Product");

            builder.HasIndex(oi => oi.IsDeleted);
        }
    }
}