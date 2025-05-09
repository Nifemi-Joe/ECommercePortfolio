// ECommercePortfolio.Core/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ECommercePortfolio.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}