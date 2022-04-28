using System;

namespace NetcoreSaas.Application.Dtos
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        Guid? CreatedByUserId { get; set; }
        DateTime? ModifiedAt { get; set; }
        Guid? ModifiedByUserId { get; set; }
    }
}