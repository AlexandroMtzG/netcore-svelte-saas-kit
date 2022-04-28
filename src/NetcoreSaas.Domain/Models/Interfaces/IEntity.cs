using System;

namespace NetcoreSaas.Domain.Models.Interfaces
{
    public interface IEntity
    {
         Guid Id { get; set; }
         DateTime CreatedAt { get; set; }
         DateTime? ModifiedAt { get; set; }
    }
}