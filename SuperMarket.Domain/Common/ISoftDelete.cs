using System;

namespace SuperMarket.Domain.Common
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTimeOffset? DeletedDate { get; set; }
        Guid? DeletedBy { get; set; }

        void SoftDelete(Guid deletedBy);
    }
}
