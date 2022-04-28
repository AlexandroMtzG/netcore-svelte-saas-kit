using System;
using NetcoreSaas.Application.Dtos.Core.Users;

namespace NetcoreSaas.Application.Dtos
{
    public class EntityDto : IAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public UserSimpleDto CreatedByUser { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ModifiedByUserId { get; set; }
        public UserSimpleDto ModifiedByUser { get; set; }
    }
}