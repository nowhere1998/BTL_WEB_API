using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTL_WEB_API.Models
{
    public class DishImage
    {
        public int DishId { get; set; }
        public string? DishName { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; } = 0;
        public int Size { get; set; } = 0;
        public IFormFile? Image { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int CategoryId { get; set; }
        public string? OldImage { get; set; }
    }
}
