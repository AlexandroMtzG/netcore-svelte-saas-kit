using System;
using NetcoreSaas.Domain.Models.Interfaces;

namespace NetcoreSaas.Domain.Models
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}