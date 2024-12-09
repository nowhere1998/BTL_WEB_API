using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTL_CALLWEB_API.Models
{
    public class Account
    {
        public int AccountId { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int Role { get; set; } = 0;
        public string? Image { get; set; }
    }
}
