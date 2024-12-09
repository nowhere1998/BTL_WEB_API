using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace BTL_CALLWEB_API.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public float TotalPrice { get; set; }
        public int status { get; set; } = 0;
        public string? note { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
