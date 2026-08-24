using System;

namespace SuperMarket.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity, ISoftDelete
    {
        public DateTimeOffset CreatedDate { get; private set; } = DateTimeOffset.UtcNow;
        public Guid? CreatedBy { get; private set; }

        public DateTimeOffset? LastModifiedDate { get; private set; }
        public Guid? LastModifiedBy { get; private set; }

        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }

        public void SetCreated(Guid createdBy)
        {
            CreatedBy = createdBy;
            CreatedDate = DateTimeOffset.UtcNow;
        }

        public void SetModified(Guid modifiedBy)
        {
            LastModifiedBy = modifiedBy;
            LastModifiedDate = DateTimeOffset.UtcNow;
        }

        public virtual void SoftDelete(Guid deletedBy)
        {
            if (IsDeleted) return;

            IsDeleted = true;
            DeletedBy = deletedBy;
            DeletedDate = DateTimeOffset.UtcNow;
        }

        public void Restore(Guid? restoredBy = null)
        {
            if (!IsDeleted) return;

            IsDeleted = false;
            DeletedBy = null;
            DeletedDate = null;

            if (restoredBy.HasValue)
                SetModified(restoredBy.Value);
        }
    }
}

