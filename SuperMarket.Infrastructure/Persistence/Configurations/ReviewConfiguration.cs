using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        private const string TableName = "Reviews";

        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ProductId)
                   .IsRequired();

            builder.Property(r => r.UserId)
                   .IsRequired();

            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(2000)
                   .IsRequired();

            builder.Property(r => r.CreatedDate).IsRequired();
            builder.Property(r => r.CreatedBy).IsRequired();
            builder.Property(r => r.LastModifiedDate);
            builder.Property(r => r.LastModifiedBy);
            builder.Property(r => r.IsDeleted).IsRequired();
            builder.Property(r => r.DeletedDate);
            builder.Property(r => r.DeletedBy);

            builder.HasOne(r => r.Product)
                   .WithMany()
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.User)
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.ProductId, r.UserId })
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("IX_Reviews_ProductId_UserId");

            builder.HasIndex(r => r.ProductId)
                   .HasDatabaseName("IX_Reviews_ProductId");

            builder.HasIndex(r => r.IsDeleted)
                   .HasDatabaseName("IX_Reviews_IsDeleted");
        }
    }
}
