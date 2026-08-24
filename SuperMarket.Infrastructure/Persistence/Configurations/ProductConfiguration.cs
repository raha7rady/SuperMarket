using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration
    : IEntityTypeConfiguration<Product>
{
    private const string TableName = "Products";

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable(TableName);

        // =========================
        // Primary Key
        // =========================

        builder.HasKey(p => p.Id);

        // =========================
        // Primitive Properties
        // =========================

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(p => p.Slug)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .IsRequired();

        // =========================
        // Auditable Properties
        // =========================

        builder.Property(p => p.CreatedDate)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .IsRequired();

        builder.Property(p => p.LastModifiedDate);

        builder.Property(p => p.LastModifiedBy);

        builder.Property(p => p.IsDeleted)
            .IsRequired();

        builder.Property(p => p.DeletedDate);

        builder.Property(p => p.DeletedBy);

        // =========================
        // Owned Types
        // =========================

        builder.OwnsOne(p => p.Title, title =>
        {
            title.Property(t => t.Value)
                .HasColumnName("Title")
                .HasMaxLength(200)
                .IsRequired();

            title.HasIndex(t => t.Value)
                .IsUnique()
                .HasDatabaseName("IX_Products_Title");
        });

        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Amount)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        });

        builder.OwnsOne(p => p.Stock, stock =>
        {
            stock.Property(s => s.Value)
                .HasColumnName("Stock")
                .IsRequired();
        });

        builder.OwnsOne(p => p.SortOrder, display =>
        {
            display.Property(d => d.Value)
                .HasColumnName("DisplayOrder")
                .IsRequired();

            display.HasIndex(d => d.Value)
                .HasDatabaseName("IX_Products_DisplayOrder");
        });

        // =========================
        // Relationships
        // =========================

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================
        // Indexes
        // =========================

        builder.HasIndex(p => p.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Products_Slug");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Products_IsActive");

        builder.HasIndex(p => p.IsDeleted)
            .HasDatabaseName("IX_Products_IsDeleted");
    }
}
