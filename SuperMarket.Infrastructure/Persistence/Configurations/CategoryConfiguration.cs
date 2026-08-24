
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        private const string TableName = "Categories";

        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(c => c.DisplayOrder)
                   .IsRequired();

            builder.Property(c => c.IsActive)
                   .IsRequired();

            builder.Property(c => c.CreatedDate).IsRequired();
            builder.Property(c => c.CreatedBy).IsRequired();
            builder.Property(c => c.LastModifiedDate);
            builder.Property(c => c.LastModifiedBy);
            builder.Property(c => c.IsDeleted).IsRequired();
            builder.Property(c => c.DeletedDate);
            builder.Property(c => c.DeletedBy);

            // ========================
            // Relationships
            // ========================
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Category)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(c => c.Products)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // ========================
            // Indexes
            // ========================
            builder.HasIndex(c => c.Title)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0");

            builder.HasIndex(c => c.DisplayOrder);
            builder.HasIndex(c => c.IsDeleted);
        }
    }
}