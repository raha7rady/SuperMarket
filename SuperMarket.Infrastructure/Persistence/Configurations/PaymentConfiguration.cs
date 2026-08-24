using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SuperMarket.Domain.Entities;

namespace SuperMarket.Infrastructure.Persistence.Configurations
{
    public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        private const string TableName = "Payments";
        private const int EnumMaxLength = 50;

        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderId)
                   .IsRequired();

            builder.Property(p => p.PaymentMethod)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(200);

            builder.Property(p => p.Description)
                   .HasMaxLength(500);

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasMaxLength(EnumMaxLength)
                   .IsRequired();

            builder.Property(p => p.CreatedDate).IsRequired();
            builder.Property(p => p.CreatedBy).IsRequired();
            builder.Property(p => p.LastModifiedDate);
            builder.Property(p => p.LastModifiedBy);
            builder.Property(p => p.IsDeleted).IsRequired();
            builder.Property(p => p.DeletedDate);
            builder.Property(p => p.DeletedBy);

            builder.Property<byte[]>("RowVersion")
                   .IsRowVersion();

            builder.OwnsOne(p => p.Amount, a =>
            {
                a.Property(x => x.Amount)
                 .HasColumnName("Amount")
                 .HasPrecision(18, 0)
                 .IsRequired();
            });

            builder.HasOne(p => p.Order)
                   .WithMany()
                   .HasForeignKey(p => p.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.OrderId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.IsDeleted);
        }
    }
}
