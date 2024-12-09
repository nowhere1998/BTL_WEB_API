using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BTL_CALLWEB_API.Models
{
    public class Dish
    {
        public int DishId { get; set; }
        public string? DishName { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; } = 0;
        public int Size { get; set; } = 0;
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
