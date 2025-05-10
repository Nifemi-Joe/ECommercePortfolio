// ECommercePortfolio.Web/ViewModels/ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ECommercePortfolio.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
    }
}