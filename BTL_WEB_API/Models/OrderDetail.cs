using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_WEB_API.Models
{
    [Table("OrderDetails")]
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Quantity { get; set; } = 1;
        public float Price { get; set; }
        [ForeignKey("DishId")]
        public Dish? Dish { get; set; }
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
    }
}
