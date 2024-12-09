using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_CALLWEB_API.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; } = 1;
        public float Price { get; set; }
    }
}
