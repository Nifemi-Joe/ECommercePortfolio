// ECommercePortfolio.Core/Entities/Category.cs
using System.Collections.Generic;

namespace ECommercePortfolio.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}