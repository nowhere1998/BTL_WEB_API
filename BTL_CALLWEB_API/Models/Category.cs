using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTL_CALLWEB_API.Models
{
    public class Category
    {
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool Status { get; set; } = true;
    }
}
