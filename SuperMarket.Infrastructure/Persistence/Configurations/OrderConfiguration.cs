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

            builder.Property(o => o.DeliveryOption)
                   .HasConversion<string>()
                   .HasMaxLength(EnumMaxLength);

            builder.Property(o => o.PaymentMethod)
                   .HasConversion<string>()
                   .HasMaxLength(EnumMaxLength);

            builder.Property(o => o.CouponCode)
                   .HasMaxLength(50);

            builder.OwnsOne(o => o.ShippingCost, cost =>
            {
                cost.Property(c => c.Amount)
                    .HasColumnName("ShippingCost")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.OwnsOne(o => o.CouponDiscount, discount =>
            {
                discount.Property(d => d.Amount)
                    .HasColumnName("CouponDiscount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.FullName)
                    .HasColumnName("Recipient_FullName")
                    .HasMaxLength(150);

                address.Property(a => a.Phone)
                    .HasColumnName("Recipient_Phone")
                    .HasMaxLength(20);

                address.Property(a => a.Province)
                    .HasColumnName("Recipient_Province")
                    .HasMaxLength(100);

                address.Property(a => a.City)
                    .HasColumnName("Recipient_City")
                    .HasMaxLength(100);

                address.Property(a => a.AddressLine)
                    .HasColumnName("Recipient_AddressLine")
                    .HasMaxLength(500);

                address.Property(a => a.PostalCode)
                    .HasColumnName("Recipient_PostalCode")
                    .HasMaxLength(20);

                address.Property(a => a.Plaque)
                    .HasColumnName("Recipient_Plaque")
                    .HasMaxLength(20);

                address.Property(a => a.Unit)
                    .HasColumnName("Recipient_Unit")
                    .HasMaxLength(20);

                address.Property(a => a.DeliveryNote)
                    .HasColumnName("Recipient_DeliveryNote")
                    .HasMaxLength(500);
            });

            builder.Property(o => o.CreatedDate).IsRequired();
            builder.Property(o => o.CreatedBy).IsRequired();
            builder.Property(o => o.LastModifiedDate);
            builder.Property(o => o.LastModifiedBy);
            builder.Property(o => o.IsDeleted).IsRequired();
            builder.Property(o => o.DeletedDate);
            builder.Property(o => o.DeletedBy);

            builder.Property<byte[]>("RowVersion")
                   .IsRowVersion();

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

            builder.HasIndex(o => o.UserId);
            builder.HasIndex(o => new { o.UserId, o.IsDeleted });
            builder.HasIndex(o => o.OrderStatus);
            builder.HasIndex(o => o.PaymentStatus);
            builder.HasIndex(o => o.IsDeleted);
        }
    }
}
