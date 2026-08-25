using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        private const string TableName = "WishlistItems";

        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId)
                   .IsRequired();

            builder.Property(w => w.ProductId)
                   .IsRequired();

            builder.Property(w => w.CreatedDate).IsRequired();
            builder.Property(w => w.CreatedBy).IsRequired();
            builder.Property(w => w.LastModifiedDate);
            builder.Property(w => w.LastModifiedBy);
            builder.Property(w => w.IsDeleted).IsRequired();
            builder.Property(w => w.DeletedDate);
            builder.Property(w => w.DeletedBy);

            builder.HasOne(w => w.User)
                   .WithMany()
                   .HasForeignKey(w => w.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.Product)
                   .WithMany()
                   .HasForeignKey(w => w.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => new { w.UserId, w.ProductId })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("IX_WishlistItems_UserId_ProductId");

            builder.HasIndex(w => w.UserId)
                   .HasDatabaseName("IX_WishlistItems_UserId");

            builder.HasIndex(w => w.IsDeleted)
                   .HasDatabaseName("IX_WishlistItems_IsDeleted");
        }
    }
}
