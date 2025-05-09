// ECommercePortfolio.Core/Entities/BaseEntity.cs
using System;

namespace ECommercePortfolio.Core.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
