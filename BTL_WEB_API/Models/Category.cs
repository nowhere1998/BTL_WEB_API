using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTL_WEB_API.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [StringLength(100)]
        public string? CategoryName { get; set; }
        public bool Status { get; set; } = true;
        public ICollection<Dish>? Dishes { get; set; } = new HashSet<Dish>();
    }
}
