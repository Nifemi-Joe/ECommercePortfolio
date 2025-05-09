// ECommercePortfolio.Web/Models/ErrorViewModel.cs
namespace ECommercePortfolio.Web.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}