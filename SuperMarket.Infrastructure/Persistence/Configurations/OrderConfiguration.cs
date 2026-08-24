using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        private const string TableName = "Orders";
        private const int EnumMaxLength = 50;

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(o => o.Id);

            builder.Property(o => o.UserId)
                   .IsRequired();

            builder.Property(o => o.OrderStatus)
                   .HasConversion<string>()
                   .HasMaxLength(EnumMaxLength)
                   .IsRequired();

            builder.Property(o => o.PaymentStatus)
                   .HasConversion<string>()
                   .HasMaxLength(EnumMaxLength)
                   .IsRequired();

            // ========================
            // Audit Fields
            // ========================

            builder.Property(o => o.CreatedDate).IsRequired();
            builder.Property(o => o.CreatedBy).IsRequired();
            builder.Property(o => o.LastModifiedDate);
            builder.Property(o => o.LastModifiedBy);
            builder.Property(o => o.IsDeleted).IsRequired();
            builder.Property(o => o.DeletedDate);
            builder.Property(o => o.DeletedBy);

            builder.Property<byte[]>("RowVersion")
                   .IsRowVersion();

            // ========================
            // Relationships
            // ========================

            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                   .WithOne(i => i.Order)
                   .HasForeignKey(i => i.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(o => o.Items)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // ========================
            // Indexes
            // ========================

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => new { o.UserId, o.IsDeleted });
            builder.HasIndex(o => o.OrderStatus);
            builder.HasIndex(o => o.PaymentStatus);
            builder.HasIndex(o => o.IsDeleted);
        }
    }
}